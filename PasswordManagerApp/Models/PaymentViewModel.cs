using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace PasswordManagerApp.Models
{
    public class PaymentsViewModel : UserRelationshipModel
    {
        public IEnumerable<PaypalAccount> PaypalAccounts { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
        
    }
}
