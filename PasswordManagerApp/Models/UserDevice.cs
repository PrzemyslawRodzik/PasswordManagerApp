
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PasswordManagerApp.Models
{   [Table("user_device")]
    public class UserDevice: UserRelationshipModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("device_guid")]
        public string DeviceGuid { get; set; }
        [Required]
        [Column("ip_address")]
        public string IpAddress { get; set; }
        [Required]
        [Column("authorized")]
        public int Authorized { get; set; }

        


    }
}
