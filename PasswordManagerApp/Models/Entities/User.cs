
using PasswordManagerApp.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordManagerApp.Models
{   [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [Column("master_password")]

        public string Password { get; set; }
        [Required]
        [Column("password_salt")]
        public string PasswordSalt { get; set; }
        [Required]
        [Column("two_factor_authorization")]
        public int TwoFactorAuthorization { get; set; }
        [Required]
        [Column("admin")]
        public int Admin { get; set; }
        [Required]
        [Column("password_notifications")]
        public int PasswordNotifications { get; set; }
        [Required]
        [Column("authentication_time")]
        public int AuthenticationTime { get; set; }

        public PersonalInfo PersonalInfo { get; set; }
        

        public ICollection<UserDevice> UserDevices { get; set; }
        public ICollection<Totp_user> Totp_Users { get; set; }
        public ICollection<LoginData> LoginDatas { get; set; }
        public ICollection<CreditCard> CreditCards { get; set; }
        public ICollection<PaypallAcount> PaypallAcounts { get; set; }
        public ICollection<Note> Notes { get; set; }
        public ICollection<SharedLoginData> SharedLoginDatas { get; set; }




    }
}
