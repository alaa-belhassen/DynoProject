using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using login.data.PostgresConn;
using login.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

using WebApplication1;

namespace login.controller
{   
    [ApiController]
    [Route("[controller]")]
    
    public class AuthentificationController:ControllerBase{
       private readonly postgres _context;
        private readonly UserManager<employer3> _usermanager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IConfiguration _configuration;
        public AuthentificationController(UserManager<employer3> _usermanager,RoleManager<IdentityRole> _RoleManager, IConfiguration _configuration,postgres context){
            this._usermanager= _usermanager;
            this._configuration = _configuration;
            this._RoleManager = _RoleManager;
            _context = context ; 
        }
///------------ Addrole methode 
        [HttpPost]
        [Route("addRole")]
        public async  Task<IActionResult> AddRole([FromBody] RoleRequestDto role) {
        if ( ModelState.IsValid)
        {
            var Role = new IdentityRole(){
                Name = role.RoleName,
            };  
            var Role_exist =await _RoleManager.RoleExistsAsync(Role.Name);
            if ( Role_exist ){
                return BadRequest("Role exist");
            }

            var is_created = await _RoleManager.CreateAsync(Role);
              if ( is_created.Succeeded ){
                //Add Token 
                return Ok("Role created");

            } else {
                List<string> errors = new List<string>();
                 foreach (var error in is_created.Errors)
                    {
                       errors.Add(error.Code + error.Description) ;
                    }
                
               return BadRequest(errors);
            
        }}
         return BadRequest();
        }
///------------ Register methode with jwtToken with Authorize control to superuser
        [HttpPost]
        [Route("Register")]
///here you set your application restrictions Roles identifies the users that can access your methode
///and you specifie that it uses JwtBearer token
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "superuser")]
        public async  Task<IActionResult> Register([FromBody] EmployerInfoDto user) {
        if ( user != null ){
            // check if user exist
            var user_exist =await  _usermanager.FindByEmailAsync(user.Email);
            
            if (user_exist != null )
                return BadRequest(new authResult(){
                    Result = false , 
                    Errors = new List<string>(){
                        "Email already Exists"
                    }    
                });
                //create new instance of the Users table model
                var new_user = new employer3() {
                    UserName = user.Name,
                    Email = user.Email,
                    EmailConfirmed = false 

                };

               

                // check if Role exist 
                var Role_exist =await _RoleManager.RoleExistsAsync(user.Role);
            IdentityResult is_created ;

        

                if (Role_exist){
            // create user 
                     is_created =await _usermanager.CreateAsync(new_user,user.Password);
                    
                }
                else{
                return BadRequest("Role does not Exist");
                }

                if (is_created.Succeeded )
                {
                    // add Employer information to database 
                    await RegisterEmployer(user);
                    // Find user by Email
                    var createdUser = await _usermanager.FindByEmailAsync(new_user.Email);

                    // add User Role
                    await _usermanager.AddToRoleAsync(createdUser, user.Role);


                    //send mail to user

                    try
                    {
                        //custom code de confirmation   
                        string code = "grdFyASkaXns3wg3XOtvr"+createdUser.Id;
                        // email link to
                        var confirmationUrl = Request.Scheme + "://" + Request.Host + @Url.Action("ConfirmEmail", "Authentification", new { email = new_user.Email, code = code });
                        //setting the email message
                        var emailBody = "<h1 style='color:#4CAF50'>Dear User</h1>" +
                            "<p>Thank you for signing up for our service. To complete the registration process, we need to verify your account.</p>" +
                            "<p> Please click on the link below to verify your email address and activate your account:</p>" +
                            "<a style='background:#4CAF50;padding: 15px 32px; text-align: center; text-decoration: none;display: inline-block;font-size: 16px;margin: 4px 2px; cursor: pointer;color:white; 'href=" + System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmationUrl) + ">Verification Link</a>" +
                            "<p>Once you've verified your account, you'll be able to start using our service right away. If you have any questions or concerns, please don't hesitate to contact our customer support team.</p>" +
                            "<p>Thank you for choosing our service!<p>";
                       
                       EmailHelperSMTP emailHelper = new EmailHelperSMTP();

                        bool emailResponse = emailHelper.SendEmail(new_user.Email, emailBody);
                        if (emailResponse)
                        {
                            return Ok("Email need confirmation");
                        }
                        return BadRequest(
                           "Log email failed"
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
               

                }
                else
                {
                    List<string> errors = new List<string>();
                    foreach (var error in is_created.Errors)
                    {
                        errors.Add(error.Code + error.Description);
                    }

                    return BadRequest(new authResult()
                    {
                        Result = false,
                        Errors = errors
                    });

                }
        

            }
       
            return BadRequest("bad");

        }





        ///------------ Add Employes Informations to MoreInfo table
        private async  Task RegisterEmployer([FromBody] EmployerInfoDto employer) {
         if (employer.Role == "employer"){
                  var user =await _usermanager.FindByEmailAsync(employer.Email);
                  var MoreInfo = new MoreInfoEmployer(){
                    IdMoreInfo = Guid.NewGuid().ToString(),
                    numeroTelEntreprise = employer.numeroTelEntreprise,
                    codeTVA = employer.codeTVA,
                    adresseFacturation =  employer.adresseFacturation,
                    AdresseEntreprise = employer.AdresseEntreprise,
                    EmailRH = employer.EmailRH ,
                    NumTelRH = employer.NumTelRH ,
                    PaymentMethode = employer.PaymentMethode,
                    moreInfoId = user.Id
                  };
                  await  _context.EmployesMoreInfo.AddAsync(MoreInfo);
                }
        }



        /// ----------------confirm email

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            if (email == null || code == null)
            {
                return BadRequest(new authResult()
                {
                    Errors = new List<string>()
              {
                  "Invalid email confirmation url"
              }
                });
            }
            var user = await _usermanager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new authResult()
                {
                    Errors = new List<string>()
              {
                  "Invalid email param"
              }
                });
            }
            string verifcode = "grdFyASkaXns3wg3XOtvr"+user.Id;
            if (code != verifcode)
            {
                BadRequest(new authResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "code incorrect"
                    }
                });
            } 
            user.EmailConfirmed = true;
            await _usermanager.UpdateAsync(user);
            return Ok("Thank you for confirming your mail") ;
        }
        // [HttpGet]
        // [Route("ConfirmEmail")]
        // [AllowAnonymous]
        // public async Task<IActionResult> ConfirmEmail(string email, string code)
        // {
        //     if (email == null || code == null)
        //     {
        //         return BadRequest(new authResult()
        //       {
        //           Errors = new List<string>()
        //{
        //       "Invalid email param"
        //   }
        //        });
        //     }
        //     var user = await _usermanager.FindByIdAsync(email);

        // }

        ///------------ Login methode with jwtToken 
        [HttpPost]
        [Route("login")]        
        public async  Task<IActionResult> Login([FromBody] UserLoginRequestDto user) {
        if ( ModelState.IsValid ){
            var user_exist =await _usermanager.FindByEmailAsync(user.Email);
            
            if ( user_exist == null)
                return BadRequest(new authResult(){
                    Errors = new List<string> (){
                        "user does not exist "
                    },
                    Result= false
                });
                if (!user_exist.EmailConfirmed)
                {
                    return BadRequest(new authResult()
                    {
                        Errors = new List<string>(){
                        "email need confirmation "
                    },
                        Result = false
                    });
                }

            var isCorrect =await _usermanager.CheckPasswordAsync(user_exist,user.Password);

            if (isCorrect){
                var token =await GenerateJwtToken(user_exist);
                return Ok(new authResult(){
                    Token = token,
                    //RefreshToken = token.RefreshToken,
                    Result = true    
                });
            }else {
                return BadRequest(new authResult(){
                    Result = false ,
                    Errors = new List<string> (){
                        "Password incorrect "
                    }   
                });
            }       
        }
        return BadRequest();
        }

///------------ List of all the claims 
        private  async  Task<List<Claim>> GetClaims(employer3 user){

                 var claims =  new List<Claim> {
                    new Claim ("Id" , user.Id),
                    new Claim (JwtRegisteredClaimNames.Sub , user.Email),
                    new Claim (JwtRegisteredClaimNames.Email , user.Email),
                    new Claim (JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                    new Claim (JwtRegisteredClaimNames.Iat , DateTime.Now.ToFileTimeUtc().ToString()),

                };
                var userClaims = await _usermanager.GetClaimsAsync(user);
                claims.AddRange(userClaims);

                var userRoles = await _usermanager.GetRolesAsync(user);
                foreach(var userRole   in userRoles){
                    
                    var Role = await _RoleManager.FindByNameAsync(userRole);
                    if (Role != null){
                    claims.Add(new Claim(ClaimTypes.Role,userRole));
                    var Roleclaims = await _RoleManager.GetClaimsAsync(Role);
                    foreach(var roleClaim   in Roleclaims){
                        claims.Add(roleClaim);
                    }
                    }
                }

            return claims;
        }


///------------ get users Roles
        [HttpGet]
        [Route("GetUsersRoutes")]
        public async Task<IActionResult> GetUsersRoutes(string email){
            
            var user = await _usermanager.FindByEmailAsync(email);
            if ( user == null ){
                return BadRequest(new {
                    error = "User does not exist "
                });
            }
            var Roles = await _usermanager.GetRolesAsync(user);
            return Ok(Roles);
        } 

///------------ Generate jwt token 
        private    async Task<string> GenerateJwtToken(employer3 user){
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var claims =await  GetClaims(user);
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:secret").Value) ;
            var TokenDescription = new SecurityTokenDescriptor (){
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)) ,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256)

            };
            Console.Write(_usermanager.GetRolesAsync(user));
            var token = JwtTokenHandler.CreateToken(TokenDescription);
            return  JwtTokenHandler.WriteToken(token); 
           /* var RefreshToken = new RefreshTokenModel(){
                Id = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                Token = GenerateRefreshToken(21), 
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false , 
                IsUsed = false ,
                UserId = user.Id
            };
           /*  await _context.RefreshTokenTable.AddAsync(RefreshToken);
            await _context.SaveChangesAsync();
           return new authResult(){
                Token = JwtToken,
                RefreshToken = RefreshToken.Token,
                Result = true
            };*/
        }

         private string GenerateRefreshToken(int length ){
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            return new string (Enumerable.Repeat(chars , length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}