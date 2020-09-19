﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace PasswordManagerApp.Handlers
{
    public  class CookieHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _protector;

        public CookieHandler(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
        {
            _httpContextAccessor = httpContextAccessor;
            _protector = provider.CreateProtector("PasswordManagerApp.CookieHandler.v1");
            
        }


        public bool CheckIfCookieExist(string key)
        {
            var cookieExist = _httpContextAccessor.HttpContext.Request.Cookies.Keys.Contains(key);

            if (cookieExist)
                return true;

            return false;
        }
        public  string ReadCookie(string key)
        {
            return  _httpContextAccessor.HttpContext.Request.Cookies[key];
        }
        public void CreateCookie(string key, string value, int? expireTime)
        {
            string protectedCookieData = "";
            string decryptedCookieData = "";
            if (CheckIfCookieExist(key))
            {
                decryptedCookieData = ReadAndDecryptCookie(key);
                value = decryptedCookieData + value;

            }
            
            
            protectedCookieData = EncryptCookieData(value);
            CookieOptions option = new CookieOptions();
            option.HttpOnly = true;
            option.SameSite = SameSiteMode.Lax;


            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddDays(365);
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, protectedCookieData, option);
           


        }
        private  string  EncryptCookieData(string cookieData)
        {

            return _protector.Protect(cookieData);

        }
        public string ReadAndDecryptCookie(string key)
        {
            //Get the encrypted cookie value
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[key];

            //Get a data protector to use with either approach
            var dataProtector = _protector;
            return dataProtector.Unprotect(cookieValue);
            

         /*
           
            //Get the decrypted cookie as a Authentication Ticket
           // TicketDataFormat ticketDataFormat = new TicketDataFormat(dataProtector);
           // AuthenticationTicket ticket = ticketDataFormat.Unprotect(cookieValue);

         */


        }
        public void RemoveCookie(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        public bool  CheckCookieData(string key, string data)
        {
            
            var cookieValue = ReadAndDecryptCookie(key);
            if (cookieValue.Contains(data))
                return false;
            return true;
        }
        public bool CheckCookieHashedData(string key, string hashedData)
        {

            var cookieValue = ReadAndDecryptCookie(key);
            SHA256 mysha256 = SHA256.Create();
            var cookieDataHash = Convert.ToBase64String(mysha256.ComputeHash(Encoding.UTF8.GetBytes(cookieValue)));
            
            if (cookieDataHash.Equals(hashedData))
                return true;
            return false;
        }

        public ClientInfo GetClientInfo()
        {
            string uaString = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            return c;
        }
    }
}
