using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public  class UserRelationshipModel
    {
        
        public int? UserId { get; set; }
        public User User { get; set; }

    }
}
