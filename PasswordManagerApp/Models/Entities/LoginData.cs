using System;
using System.ComponentModel.DataAnnotations;


namespace PasswordManagerApp.Models.Entities
{
    public class LoginData
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        [Required]
        public string Login { get; set; }
        public string Website { get; set; }
        [Required]
        public int Compromised { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }


        public User User { get; set; }





    }
}
