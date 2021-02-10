using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;


namespace PasswordManagerApp.Models
{
    public class Totp_user : UserRelationshipModel
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        [Column("token")]
        public string Token { get; set; }


        [Required]
        [Column("create_date")]
        public DateTime Create_date { get; set; }
        [Required]
        [Column("expire_date")]
        public DateTime Expire_date { get; set; }

        

        
        
        
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
