using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PasswordGenerator;
using PasswordManagerApp.ApiResponses;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{   
    [Authorize]
    [Route("settings")]
    public class SettingsController : Controller
    {
       // private readonly IUserService _userService;
        private readonly ApiService _apiService;
        private readonly JwtHelper _jwtHelper;

        public SettingsController(ApiService apiService, JwtHelper jwtHelper)
        {
             _apiService = apiService;
            _jwtHelper = jwtHelper;
        }

        public IActionResult Index()
        {
            PopulateForm();

           
            return View();
        }
        private void  PopulateForm()
        {
            ViewData["2F"] = HttpContext.User.FindFirst("TwoFactorAuth").Value;
            ViewData["PassNot"] = HttpContext.User.FindFirst("PasswordNotifications").Value;
            ViewData["valueTime"] = HttpContext.User.FindFirst("AuthTime").Value;
        }






        [HttpGet]
        [Route("passwordgenerate")]
        public IActionResult PasswordGenerator()
        {

            ViewBag.Password = "";
            return PartialView("PasswordGenerator", new PassGeneratorViewModel());
        }


        [HttpPost]
        [Route("passwordgenerate")]
        public IActionResult PasswordGenerator(PassGeneratorViewModel model)
        {
            string result;

            if (!(model.Length >= 8 && model.Length <= 128))
                model.Length = 16;

            if (model.IncludeLowercase == false && model.IncludeUppercase == false && model.IncludeSpecial == false && model.IncludeNumeric == false)
            {
                result = new Password().LengthRequired(model.Length).Next();
            }
            else
            {
                var pwd = new Password(model.IncludeLowercase, model.IncludeUppercase, model.IncludeNumeric, model.IncludeSpecial, model.Length);

                result = pwd.Next();
            }


            ViewBag.Password = result;
            return PartialView("PasswordGenerator", model);





        }
       
        [HttpGet]
        [Route("passwordchange")]

        public IActionResult PasswordChange() => PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
           
             
       

        [Route("passwordchange")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)  
        {
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Change highlighted fields.");
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
           ApiResponse apiResponse =  await _apiService.ChangeMasterPassword(model);
            
             if(apiResponse.Success)
                {
                    ViewBag.Message = apiResponse.Messages.First();
                    return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
                }
            else
            {
                ModelState.AddModelError("Error", apiResponse.Messages.First());
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
        }





        [HttpPost]
        [Route("updatepreferences")]
        public async Task<IActionResult> UpdatePreferences(string switch2F,string switchPnot, string sliderVerTime)
        {
            

            var apiResponse = await _apiService.UpdateUserPreferences(switch2F, switchPnot, sliderVerTime);


            if (apiResponse.Success)
            {
                await _jwtHelper.ValidateTokenAndSignIn(apiResponse.AccessToken);
                ViewBag.Message = apiResponse.Messages.First();
                return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            }
            else
            {
                ViewBag.Message = apiResponse.Messages.First();
                return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
            }
        }

        
        
              
            
            
            
        
        







    }
}