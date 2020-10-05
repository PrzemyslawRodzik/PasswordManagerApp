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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;

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
            ViewBag.UserEmail = user.Email;
            return View("Views/Wallet/IndexDashboard.cshtml");

        }


        
        
        
        
        



       
        
       









    }
}