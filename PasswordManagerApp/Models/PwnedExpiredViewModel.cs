using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace PasswordManagerApp.Models
{
    public class PwnedExpiredViewModel : UserRelationshipModel
    {
        public IEnumerable<LoginData> LoginDatasBreached { get; set; }
        public IEnumerable<LoginData> LoginDatasExpired { get; set; }
        public IEnumerable<PaypalAccount> PaypalAccountsBreached { get; set; }
        public IEnumerable<PaypalAccount> PaypalAccountsExpired { get; set; }

    }
}
