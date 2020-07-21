
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PasswordManagerApp.Models
{   [Table("user_device")]
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceGuid { get; set; }
        [Required]
        public int Authorized { get; set; }

        
        public User User { get; set; }


    }
}
