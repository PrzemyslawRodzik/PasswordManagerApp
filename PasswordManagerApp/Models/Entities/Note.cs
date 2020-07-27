using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.Entities
{
    [Table("note")]
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("details")]
        public string Details { get; set; }

        
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
