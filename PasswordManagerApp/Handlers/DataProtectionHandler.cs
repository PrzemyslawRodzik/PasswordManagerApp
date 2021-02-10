using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Handlers
{
    public class DataProtectionHelper
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public DataProtectionHelper(IDataProtectionProvider provider)

        {
            _dataProtectionProvider = provider;
        }
        public string Encrypt(string textToEncrypt, string key)
        {
            if (textToEncrypt is null)
                return textToEncrypt;
            return _dataProtectionProvider.CreateProtector(key).Protect(textToEncrypt);

        }
        public string Decrypt(string cipherText, string key)
        {
            if (cipherText is null)
                return cipherText;
            return _dataProtectionProvider.CreateProtector(key).Unprotect(cipherText);

        }
    }
}
