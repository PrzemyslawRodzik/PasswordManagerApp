using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    [Table("shared_login_data")]
    public class SharedLoginData: UserRelationshipModel
    {   [Key]
        public int Id { get; set; }

        [Column("login_data_id")]
        public int LoginDataId { get; set; }
        public LoginData LoginData { get; set; }

       

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; }
    }
}
