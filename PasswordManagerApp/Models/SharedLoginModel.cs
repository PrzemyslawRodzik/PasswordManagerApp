

using System;

namespace PasswordManagerApp.Models
{

    public class SharedLoginModel
    {

        public LoginData LoginData { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
