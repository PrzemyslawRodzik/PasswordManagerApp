using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Models
{
    public class UpdatePreferencesWrapper
    {
        public UpdatePreferencesWrapper(string twoFactor, string pNot,string verTime)
        {
            TwoFactor = twoFactor;
            PassNotifications = pNot;
            VerificationTime = verTime;
        }
        
        
    
        public string VerificationTime { get; set; }
        private string _twoFactor;
        private string _passNotifications;
        public string TwoFactor
        {
            get => _twoFactor;
            set
            {
                _twoFactor = ConvertValue(value);
            }




        }
        public string PassNotifications
        {
            get => _passNotifications;
            set
            {
                _passNotifications = ConvertValue(value);
            }




        }

        private string ConvertValue(string value)
        {
            
            if (!(value is null))
                return "1";
            else
                return "0";
        }
    }

}
