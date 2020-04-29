using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{   [Table("user_device")]
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CookieDeviceHash { get; set; }
        [Required]
        public int Authorized { get; set; }

        
        public User User { get; set; }


    }
}
