using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.ApiResponses
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
