using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{   [Table("visitor_agent")]
    public class VisitorAgent
    {   [Key]
        public int Id { get; set; }
       
        [Required]
        [Column("browser")]
        public string Browser { get; set; }
        [Required]
        [Column("operating_system")]
       public string OperatingSystem { get; set; }
        
        [Required]
        [Column("country")]
        public string Country { get; set; }
        [Required]
        [Column("visit_time")]
        public string VisitTime { get; set; }
    }
}
