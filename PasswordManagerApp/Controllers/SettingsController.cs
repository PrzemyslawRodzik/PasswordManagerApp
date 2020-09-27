using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PasswordGenerator;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{   
    [Authorize]
    [Route("settings")]
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;
        public SettingsController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            PopulateForm();

           
            return View();
        }
        private void PopulateForm()
        {
            var authUser = _userService.GetById(Int32.Parse(HttpContext.User.Identity.Name) );
            ViewData["2F"] = authUser.TwoFactorAuthorization.ToString();
            ViewData["PassNot"] = authUser.PasswordNotifications.ToString();
            ViewData["valueTime"] = authUser.AuthenticationTime.ToString();
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
        public IActionResult PasswordChange(PasswordChangeViewModel model)  
        {
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Change highlighted fields.");
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
                
           
                 
            var authUserId = HttpContext.User.Identity.Name;
             bool isSuccess =  _userService.ChangeMasterPassword(model.NewPassword,authUserId);
            
             if(isSuccess)
                {
                    ViewBag.Message = "Password has successfully changed.";
                    return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
                }
            else
            {
                ModelState.AddModelError("Error", "Something goes wrong. Try again later.");
                return PartialView("~/Views/Auth/PasswordChange.cshtml", new PasswordChangeViewModel());
            }
                
                
         

        }

        [HttpPost]
        [Route("updatepreferences")]
        public IActionResult UpdatePreferences(string switch2F,string switchPnot, string sliderVerTime)
        {

            int authUserId = Int32.Parse(HttpContext.User.Identity.Name);
            _userService.UpdatePreferences(new UpdatePreferencesWrapper(switch2F, switchPnot, sliderVerTime), authUserId);
            

          
            ViewBag.Message = "Changes was saved";
            return PartialView("~/Views/Shared/_NotificationAlert.cshtml");
        }

        
        
              
            
            
            
        
        







    }
}