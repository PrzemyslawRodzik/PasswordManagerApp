
using PasswordManagerApp.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


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
        [Required]
        public int TwoFactorAuthorization { get; set; }
        [Required]
        public int Admin { get; set; }



        public ICollection<UserDevice> UserDevices { get; set; }
        public ICollection<Totp_user> Totp_Users { get; set; }
        public ICollection<LoginData> LoginDatas { get; set; }



    }
}
