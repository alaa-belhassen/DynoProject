using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace login.models
{
    public class RefreshTokenModel{

        public string Id {get;set;}
        public string UserId {get;set;}
        public string Token {get;set;}
        public string JwtId {get;set;}
        public bool IsUsed {get;set;}
        public bool IsRevoked {get;set;}
        public DateTime AddedDate {get;set;}
        public DateTime ExpiryDate {get;set;}
           
    }
}