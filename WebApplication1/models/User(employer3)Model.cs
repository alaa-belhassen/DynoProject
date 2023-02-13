using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace login.models
{
    public class employer3 :IdentityUser {

     
        public MoreInfoEmployer moreInfo {get;set;}    
    }
}