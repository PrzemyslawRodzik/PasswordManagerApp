using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.ApiResponses
{
    public class ApiTwoFactorResponse : AuthResponse
    {
        public int VerificationStatus { get; set; }
        
    }
}
