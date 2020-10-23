using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models.ViewModels
{
    public class PassGeneratorViewModel
    {
        public bool IncludeLowercase { get; set; }
        public bool IncludeUppercase { get; set; }
        public bool IncludeSpecial { get; set; }
        public bool IncludeNumeric { get; set; }
        public int Length { get; set; }



    }
}
