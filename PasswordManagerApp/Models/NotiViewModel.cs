using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class NotiViewModel:UserRelationshipModel
    {
        
            public IEnumerable<PaypalAccount>  PaypalAccounts { get; set; }
            public IEnumerable<LoginData> LoginDatas { get; set; }
           
        
    }
}
