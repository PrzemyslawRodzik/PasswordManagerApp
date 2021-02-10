using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public interface ICompromisedModel
    {
        public string Name { get; set; }

        public int Compromised { get; set; }
        public int? UserId { get; set; }



    }
}
