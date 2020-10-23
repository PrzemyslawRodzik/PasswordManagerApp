using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class NoteViewModel
    {
        
        

        [Required]
        [StringLength(45, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public string Details { get; set; }

        
    }
}
