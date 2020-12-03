using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordManagerApp.Models
{
    public class ShareLoginModel
    {
        public LoginData LoginData { get; set; }
        public string ReceiverEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
