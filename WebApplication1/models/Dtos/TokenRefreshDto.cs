using System.ComponentModel.DataAnnotations;

namespace login.models
{
    public class RefreshTokenDto{
        [Required]
        public string Token{get;set;}

        [Required]
        public string RefreshToken{get;set;}

    }

}