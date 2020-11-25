using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class Expired: UserRelationshipModel
    {
        public int Id { get; set; }
        public  string name { get; set; }
        public  DateTime dateTime { get; set; }
    }
}

