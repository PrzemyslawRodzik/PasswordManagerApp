using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Models;
using EmailService;

namespace PasswordManagerApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;

        }

        public IActionResult Index()
        {
            
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
            return Ok(DateTime.UtcNow.ToLocalTime());
        }


    }
}
