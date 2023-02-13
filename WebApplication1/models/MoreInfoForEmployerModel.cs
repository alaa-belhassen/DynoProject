using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace login.models
{
    public class MoreInfoEmployer  {
        [Key]
        public string IdMoreInfo {get;set;}
        public string codeTVA {get;set;}
        public string AdresseEntreprise {get;set;}
        public string EmailRH {get;set;}
        public string NumTelRH {get;set;}
        public string PaymentMethode {get;set;}
        public string moreInfoId {get;set;}  
        public employer3 employer {get;set;}
        public string numeroTelEntreprise {get;set;}
        public string adresseFacturation {get;set;}
  
    }
}