using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordManagerApp.Models
{
    
    public class LoginData: UserRelationshipModel,ISearchable,ICompromisedModel,IPasswordModel
    {

        [Key]
        public int Id { get; set; }
        [Required]
        
        public string Name { get; set; }
       
       
        public string Email { get; set; }
        [Required]
       
        public string Login { get; set; }

        [Required]
       
        public string Password { get; set; }
       
        public string Website { get; set; }
        [Required]
       
        public int Compromised { get; set; }
        [Required]
      
        public int OutOfDate { get; set; } = 0;
        [Required]
       
        public DateTime ModifiedDate { get; set; }

       


        public ICollection<SharedLoginData> SharedLoginDatas { get; set; }





    }
}
