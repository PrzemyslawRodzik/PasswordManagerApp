using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace PasswordManagerApp.Handlers
{
    public class LogInHandler
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieHandler cookieHandler;
        public ApiService _apiService;
       

        public LogInHandler(IDataProtectionProvider provider, IHttpContextAccessor httpContextAccessor, ApiService apiService,IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            cookieHandler = new CookieHandler(_httpContextAccessor, provider,config);
            _apiService = apiService;
        }

        
        public async Task LogInUser(ClaimsPrincipal claimsPrincipal, AuthenticationProperties authProperties)
        {

            _ = _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            HandleNewDeviceLogIn(Int32.Parse(claimsPrincipal.Identity.Name));
            
            await Task.CompletedTask;

        }
        



        public void HandleNewDeviceLogIn(int userId)
        {
            var c = cookieHandler.GetClientInfo();
            string browser = c.UA.Family.ToString() + " " + c.UA.Major.ToString();
            string osName = c.OS.ToString();
            string guidDevice = "";
            bool IsUserGuidDeviceMatch = true;
            var deviceCookieExist = cookieHandler.CheckIfCookieExist("DeviceInfo");
            if (deviceCookieExist)
            {
                var GuidDeviceFromCookie = cookieHandler.ReadAndDecryptCookie("DeviceInfo");
                IsUserGuidDeviceMatch = _apiService.CheckUserGuidDeviceInDb(
                    dataToSHA256(GuidDeviceFromCookie),userId); 
                if (IsUserGuidDeviceMatch)
                    return;
            }
            if(deviceCookieExist==false || IsUserGuidDeviceMatch==false)
            {
               guidDevice = Guid.NewGuid().ToString();
               cookieHandler.CreateCookie("DeviceInfo", guidDevice, null);
               _apiService.HandleNewDeviceLogIn(GetUserIpAddress(), 
                   dataToSHA256(guidDevice),userId,osName, browser);
            }
        }
        private string GetUserIpAddress() => _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        private string dataToSHA256(string data)
        {
            SHA256 mysha256 = SHA256.Create();
            return Convert.ToBase64String(mysha256.ComputeHash(Encoding.UTF8.GetBytes(data)));

        }

    }
}
