using Bogus.DataSets;
using PasswordManagerApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class Not_comp: UserRelationshipModel
    {
        public int Id { get; set; }
        public string name;
        public int Compromised;

    }
}
