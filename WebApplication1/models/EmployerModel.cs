using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace login.models
{
    public class Customer :IdentityUser {

        public string numeroTelEntreprise {get;set;}
        public string adresseFacturation {get;set;}
        public string codeTVA {get;set;}
        public string AdresseEntreprise {get;set;}
        public string EmailRH {get;set;}
        public string NumTelRH {get;set;}
        public string PaymentMethode {get;set;}
        
        public MoreInfoEmployer moreInfo {get;set;}    
    }
}