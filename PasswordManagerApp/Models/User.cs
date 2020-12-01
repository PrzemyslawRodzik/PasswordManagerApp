
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordManagerApp.Models
{   
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
       
        public string Email { get; set; }
        [Required]
        

        public string Password { get; set; }
        [Required]
        
        public string PasswordSalt { get; set; }

        public int Compromised { get; set; }
        [Required]
       
        public int TwoFactorAuthorization { get; set; }
        [Required]
       
        public int PasswordNotifications { get; set; }
        [Required]
       
        public int AuthenticationTime { get; set; }
    

    
        public PersonalInfo PersonalInfo { get; set; }
        

        public ICollection<UserDevice> UserDevices { get; set; }
        public ICollection<Totp_user> Totp_Users { get; set; }
        public ICollection<LoginData> LoginDatas { get; set; }
        public ICollection<CreditCard> CreditCards { get; set; }
        public ICollection<PaypalAccount> PaypalAccounts { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<SharedLoginData> SharedLoginDatas { get; set; }




    }
}
