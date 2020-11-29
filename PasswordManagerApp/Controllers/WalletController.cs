using Bogus.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PasswordGenerator;

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
using PasswordManagerApp.Services;
using System.Security.Claims;
using PasswordManagerApp.Cache;

namespace PasswordManagerApp.Controllers
{  
    [Authorize]   
    [Route("user")]
    public class WalletController : Controller
    {
        
        private readonly ApiService _apiService;
        private readonly ICacheService _cache;
        

        public WalletController(ApiService apiService, ICacheService cache)
        {
            _apiService = apiService;
            _cache = cache;
            
        }

        [Route("dashboard")]
        [Route("~/")]
        public async Task<IActionResult> Index()   
        {
                
            var dictionary = await _cache.GetOrCreateCachedResponse<Dictionary<string,int>>(CacheKeys.Statistics + HttpContext.User.Identity.Name, () => _apiService.GetUserStatisticData());
            ViewBag.StatisticData = dictionary;
            ViewBag.UserEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value;

            
            return View("Views/Wallet/IndexDashboard.cshtml");

        }
        public PartialViewResult GetData()
        {

            ViewBag.UserEmail= HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value;
            return PartialView("~/Views/Shared/EmailPartialView.cshtml");
        }
        [HttpGet]
        public IActionResult GetEmail(string email)
        {   
            email = ViewBag.UserEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value;
            return PartialView(email);
        }



    }
}