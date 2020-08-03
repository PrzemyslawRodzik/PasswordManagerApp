using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{   [Table("phone_number")]
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("nickname")]
        public string NickName { get; set; }
        [Required]
        [Column("phone_number")]
        public string TelNumber { get; set; }
        [Required]
        [Column("type")]
        public string Type { get; set; }
        [Column("personal_info_id")]
        public int PersonalInfoId { get; set; }
        public PersonalInfo PersonalInfo { get; set; }


    }
}
