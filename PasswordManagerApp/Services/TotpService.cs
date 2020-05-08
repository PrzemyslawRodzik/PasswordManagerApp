using Microsoft.AspNetCore.Identity;
using OtpNet;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public class TotpService
    {
        private readonly ApplicationDbContext _db;


        public Totp totp { get; private set; }
        private  string totpToken { get;}
        public TotpService(ApplicationDbContext db)
        {
           
        }

        public TotpService(string key, OtpHashMode hashMode = OtpHashMode.Sha512,int step=300)
        {
            var key_b = Encoding.UTF8.GetBytes(key);
            totp = new Totp(secretKey:key_b,mode:hashMode,step:300);
            totpToken = totp.ComputeTotp(DateTime.UtcNow);
        }
        public void SaveTokenToDb(User authUser)
        {
            
        }
        





    }
}
