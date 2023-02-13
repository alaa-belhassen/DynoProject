using System.ComponentModel.DataAnnotations;

namespace login.models
{
    public class UserLoginRequestDto{
        [Required]
        public string Password{get;set;}
        [Required]
        public string Email{get;set;}

    }

}