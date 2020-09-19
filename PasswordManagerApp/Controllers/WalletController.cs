using Bogus.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PasswordGenerator;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Handlers;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Security.Cryptography;
using Quartz;
using System.Threading.Tasks;
using Quartz.Impl;

namespace PasswordManagerApp.Controllers
{  
    [Authorize]   // pamiętać by odkomentować !
    [Route("user")]
    public class WalletController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
            public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        [Route("dashboard")]
        [Route("~/")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(controllerName: "Auth", actionName: "Login");


            var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));

            var countLogin = _unitOfWork.Wallet.GetDataCountForUser<LoginData>(user);
            var countCreditCards = _unitOfWork.Wallet.GetDataCountForUser<CreditCard>(user);

            ViewBag.countCreditCards = countCreditCards;

          var countPaypall =  _unitOfWork.Wallet.GetDataCountForUser<PaypallAcount>(user);

            ViewBag.countPasswords = countLogin + countPaypall;

           var countPaypalComp =  _unitOfWork.Wallet.GetDataBreachForUser<PaypallAcount>(user);

           var countLoginComp =  _unitOfWork.Wallet.GetDataBreachForUser<LoginData>(user);
            ViewBag.countCompromised = countPaypalComp + countLoginComp;

            ViewBag.countSharedData = _unitOfWork.Wallet.GetDataCountForUser<SharedLoginData>(user);

            return View("Views/Wallet/IndexDashboard.cshtml");

        }


        [Route("list")]
        public IActionResult List()
        {

            var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));
            var loginData = _unitOfWork.Wallet.FindByCondition<LoginData>(x => x.User == user);

            return View("Views/Wallet/ListItem.cshtml", loginData);
        }
        /* [HttpGet("{errorType}")]
         public IActionResult Error(string errorType)
         {

             return View("Views/Shared/"+errorType+".cshtml");

         }

     */


        
        


        [Route("create")]
        [HttpGet]
        public IActionResult LoginDataCreate()
        {
            return View(new LoginDataViewModel());
        }
        [Route("create")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LoginDataCreate(LoginDataViewModel model)
        {
            if (!ModelState.IsValid)

                return View(model);



            return Ok("true");



        }
        // do LoginData controller !!!


        [Route("/ip")]
        public IActionResult Ip()
        {
            //string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            

            var data = System.Text.Encoding.UTF8.GetBytes("anrzej duda");
            //    var (publicKey, privateKey) = AsymmetricEncryptionHelper.GenerateKeys("przemek");
            //  var pubString = Convert.ToBase64String(publicKey.);
            //  var privString = Convert.ToBase64String(privateKey);
            var user = _unitOfWork.Users.Find<User>(203);
            var encryptedData =   AsymmetricEncryptionHelper.Encrypt(data, user.PublicKey);
            var encString = Convert.ToBase64String(encryptedData);
            var decryptedData = AsymmetricEncryptionHelper.Decrypt(encryptedData, user.PrivateKey,"Farmer1998@");
            var decString = System.Text.Encoding.UTF8.GetString(decryptedData);
            
            
            






            return Ok(ipAddress);
        }
        [Route("/date")]
        public IActionResult Date(){
          
           var data = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd' 'HH:mm:ss");
          
           // DateTime.Now.ToString("HH:mm:ss tt");
            return Ok(data);
        }
       









    }
}