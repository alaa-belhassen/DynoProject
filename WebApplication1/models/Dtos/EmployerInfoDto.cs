using System.ComponentModel.DataAnnotations;

namespace login.models
{
    public class EmployerInfoDto :UserRegistrationRequestDto{
        
        public string numeroTelEntreprise {get;set;}
        public string adresseFacturation {get;set;}
        public string codeTVA {get;set;}
        public string AdresseEntreprise {get;set;}
        public string EmailRH {get;set;}
        public string NumTelRH {get;set;}
        public string PaymentMethode {get;set;}

    }

}