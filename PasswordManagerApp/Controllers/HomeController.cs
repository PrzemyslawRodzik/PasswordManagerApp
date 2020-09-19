using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Models.ViewModels;
using EmailService;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Routing;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using PasswordManagerApp.Interfaces;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using PasswordGenerator;

namespace PasswordManagerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtectionProvider _provider;
        public CookieHandler cookieHandler;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender,IUnitOfWork unitOfWork, IDataProtectionProvider provider)
        {
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _provider = provider;
            cookieHandler = new CookieHandler(new HttpContextAccessor(), _provider);

        }

        
        [Route("welcome")] // strona powitalna
        public  IActionResult Index()
        {

            
            VisitorAgentStatistics();
            
            
            
                
            ViewBag.LogoutMessage = TempData["logoutMessage"] as string;
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
            
        }
        
        [Route("email")]
        [HttpGet]
        public async Task Email()
        {
            var message = new Message(new string[] { "przemyslawrodzik@gmail.com" }, "Tytul emailu asynchronicznego", "This is the content from our async email.");
            await _emailSender.SendEmailAsync(message);


            
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DateNow()
        {
            var pwd = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial().LengthRequired(18);
            var result = pwd.Next();

            return Ok(result);
           
        }

        private void VisitorAgentStatistics()
        {
            bool cookieVisitorExist = cookieHandler.CheckIfCookieExist("VisitorCookie");
            bool cookieDeviceExist = cookieHandler.CheckIfCookieExist("DeviceInfo");
            if (cookieVisitorExist || cookieDeviceExist)
                return;
            cookieHandler.CreateCookie("VisitorCookie", Guid.NewGuid().ToString(),null);
            var countryName = GetVisitorLocationAsync(Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            var c = cookieHandler.GetClientInfo();
            VisitorAgent visitor = new VisitorAgent();
            visitor.Browser = c.UA.Family.ToString().ToString() + " " + c.UA.Major.ToString();
            visitor.OperatingSystem = c.OS.Family.ToString();
            visitor.Country = countryName.Result;
            visitor.VisitTime = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd' 'HH:mm:ss");
          
            _unitOfWork.Context.Add<VisitorAgent>(visitor); 
            _unitOfWork.SaveChanges();

        }
        private async Task<string> GetVisitorLocationAsync(string ip)
        {
            string apiKey = "d9c08ff5970ecf16de9952e42bcc620f";
            
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
