using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Models.ViewModels;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Routing;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using PasswordGenerator;
using Microsoft.Extensions.Hosting;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{
    public class HomeController : Controller
    {
        
        
        private readonly IDataProtectionProvider _provider;
        public CookieHandler cookieHandler;
        private readonly ApiService _apiService;
        private readonly IConfiguration _config;
        private readonly EncryptionService _mock;

        public HomeController(ApiService apiService,IDataProtectionProvider provider,IConfiguration config, EncryptionService mock)
        {
            
            
            _provider = provider;
            cookieHandler = new CookieHandler(new HttpContextAccessor(), _provider,config);
            _apiService = apiService;
            _config = config;
            _mock = mock;
            

        }

        
        [Route("welcome")] // strona powitalna
        public  IActionResult Index()
        {
            
            VisitorAgentStatistics();
            
            
            
                
            ViewBag.LogoutMessage = TempData["logoutMessage"] as string;
            return View();
        }

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

      

        private async void VisitorAgentStatistics()
        {
            bool cookieVisitorExist = cookieHandler.CheckIfCookieExist("VisitorCookie");
            bool cookieDeviceExist = cookieHandler.CheckIfCookieExist("DeviceInfo");
            if (cookieVisitorExist || cookieDeviceExist)
                return;
            cookieHandler.CreateCookie("VisitorCookie", Guid.NewGuid().ToString(),null);
            var countryName = GetVisitorLocationAsync(Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            var c = cookieHandler.GetClientInfo();
            VisitorAgent visitor = new VisitorAgent();
            visitor.Browser = c.UA.Family + " " + c.UA.Major;
            visitor.OperatingSystem = c.OS.Family+" "+  c.OS.Major;
            visitor.Country = countryName.Result;
            visitor.VisitTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd' 'HH:mm:ss");
            
            await _apiService.CreateUpdateData<VisitorAgent>(visitor);

        }
        private async Task<string> GetVisitorLocationAsync(string ip)
        {
            string apiKey = _config["IpLocationApiKey"];
            
            string countryName = "Unknown";
            string requestUri = $"http://api.ipstack.com/{ip}?access_key={apiKey}";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage httpResponse = await httpClient.GetAsync(requestUri))
                    {
                        if (httpResponse != null && httpResponse.IsSuccessStatusCode)
                        {
                            string apiResponse = await httpResponse.Content.ReadAsStringAsync();
                            JObject jObject = JObject.Parse(apiResponse);
                            
                            countryName = jObject.SelectToken("country_name").ToString();
                            if (countryName.Equals(""))
                                countryName = "Unknown";
                        }
                    }
                    return countryName;
                }
                catch (HttpRequestException)
                {
                    return countryName;
                }
            }
        }


    }
}
