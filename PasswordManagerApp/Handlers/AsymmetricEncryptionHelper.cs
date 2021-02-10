using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PasswordManagerApp.Handlers
{
    public static class AsymmetricEncryptionHelper
    {
       

        public static (string publicKey, string privateKey) GenerateKeys(string password, int keyLength = 2048)
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = keyLength;
                PbeParameters pbe = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc,
                   HashAlgorithmName.SHA256, 10000);

                var encPrivateKey = rsa.ExportEncryptedPkcs8PrivateKey(System.Text.Encoding.UTF8.GetBytes(password), pbe);
                var publicKey = rsa.ExportRSAPublicKey();

                return ( publicKey:  Convert.ToBase64String(publicKey),
                    
                    privateKey:  Convert.ToBase64String(encPrivateKey)
                );
            }
        }

        public static byte[] Encrypt(byte[] data, string publicKey)
        {   
            using (var rsa = RSA.Create())
            {
                
               
                int bytesread;
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey),out bytesread);

                var result = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                return result;
            }
        }

        public static byte[] Decrypt(byte[] data, string privateKey,string password)
        {
            using (var rsa = RSA.Create())
            {
                int bytesread;
                
                rsa.ImportEncryptedPkcs8PrivateKey(System.Text.Encoding.UTF8.GetBytes(password), Convert.FromBase64String(privateKey), out bytesread);

                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            }
        }


    }
}
