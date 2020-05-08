using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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



    }
}
