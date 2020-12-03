using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{

    public class EncryptionService
    {
        

        
        private readonly Dictionary<string, byte[]> _userSymmetricKeys;

        public EncryptionService()
        {
            
            _userSymmetricKeys = new Dictionary<string, byte[]>();
        }

        public Dictionary<string, byte[]> GetUserSymmetricKeys()
        {
            return _userSymmetricKeys;
        }
        public byte[] GetEncryptionKeyById(string userId)
        {
            
            return _userSymmetricKeys.FirstOrDefault(x => x.Key.Equals(userId)).Value;
        }
        public void AddOrUpdateEncryptionKey(string userId, string password)
        {
            var symmetric_key = dataToSHA256(password);
            if (_userSymmetricKeys.ContainsKey(userId))
                _userSymmetricKeys.Remove(userId);

            _userSymmetricKeys.Add(userId, symmetric_key);
            

        }   
        public void RemoveEncryptionKey(string userId)
        {
            if (_userSymmetricKeys.ContainsKey(userId))
                _userSymmetricKeys.Remove(userId);
        }
        public void PurgeDictionary()
        {
            _userSymmetricKeys.Clear();
        }

        public string[] GetActiveUsers() => _userSymmetricKeys.Keys.ToArray();



        public string Encrypt(string userId, string data) 
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = GetEncryptionKeyById(userId);
                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;
                
                return AESHelper.EncryptAES(data, myAes.Key);
            }   
        }

            

            
        
        public string Decrypt(string userId, string encData)
        {
            using (Aes myAes = Aes.Create())
            {   myAes.Key = GetEncryptionKeyById(userId);
                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;

                return AESHelper.DecryptAES(encData, myAes.Key);
            }   
        }
        public string DecryptSharedData(string key, string encData)
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = dataToSHA256(key);
                myAes.Mode = CipherMode.CBC;
                myAes.Padding = PaddingMode.PKCS7;

                return AESHelper.DecryptAES(encData, myAes.Key);
            }
        }




        private string ToBase64String(byte[] data) => Convert.ToBase64String(data);
        private byte[] dataToSHA256(string data)
        {
            SHA256 mysha256 = SHA256.Create();
            return mysha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
        


    }




    

    


    
}

