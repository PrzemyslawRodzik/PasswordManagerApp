using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PasswordManagerApp.Handlers
{
    class AESHelper
    {
        
        public static string EncryptAES(string plainText, byte[] Key)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                    encrypted = encrypted.Concat(aesAlg.IV).ToArray();
                }  
            }


            return Convert.ToBase64String(encrypted);
        }
        private static byte[] GetEncPart(byte[] byteArray)
        {
            byte[] tmp = new byte[byteArray.Length-16];
            
            Array.Copy(byteArray, tmp, byteArray.Length-16);

            return tmp;
        }
        private static byte[] GetIVPart(byte[] byteArray)
        {
            byte[] tmp = new byte[16];
            Array.Copy(byteArray, byteArray.Length-16,tmp, 0,16);

            return tmp;
        }

       public static string DecryptAES(string cipherText, byte[] Key)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var cipherDataPart = GetEncPart(cipherTextBytes);
            byte[] IV = GetIVPart(cipherTextBytes);
            
            if (cipherTextBytes == null || cipherTextBytes.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            
            string plaintext = null;

            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherDataPart))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
