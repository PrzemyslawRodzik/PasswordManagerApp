using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    
    public class SharedLoginData: UserRelationshipModel
    {   [Key]
        public int Id { get; set; }

        
        public int LoginDataId { get; set; }
        public LoginData LoginData { get; set; }

       

        [Required]
        
        public DateTime StartDate { get; set; }

        [Required]
       
        public DateTime EndDate { get; set; }
    }
}
