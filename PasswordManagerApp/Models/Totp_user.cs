using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class Totp_user
    {
        [Key]
        public int Id { get; set; }

        private string _token;  
        [Required]
        public string Token
        {
            get => _token;
            set
            {
                _token = TotpHashBase64(value);
            }




        }
        [Required]
        public int Active { get; set; } = 1;
        [Required]
        public DateTime Create_date { get; set; }
        [Required]
        public DateTime Expire_date { get; set; }

        public User User { get; set; }

        
        
        
        public static string TotpHashBase64(string token)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {

                var hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(token));
                return Convert.ToBase64String(hash);

            }
        }
    }
}
