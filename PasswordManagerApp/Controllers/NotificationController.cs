using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Cache;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;


using System.Net.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PasswordManagerApp.Models.ViewModels;


namespace PasswordManagerApp.Controllers
{
    public class NotificationController : Controller
    {

        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        public DataProtectionHelper dataProtectionHelper;
        public NotificationController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
        }

        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }


        [Route("notification")]
        public async Task<IActionResult> IndexAsync()
        {

            var userId = HttpContext.User.Identity.Name;
            NotiViewModel NotiModel = new NotiViewModel();

            try
            {
                NotiModel.PaypalAccounts = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccount + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                NotiModel.PaypalAccounts = Enumerable.Empty<PaypalAccount>();
            }
            NotiModel.PaypalAccounts = NotiModel.PaypalAccounts ?? Enumerable.Empty<PaypalAccount>();

            try
            {
                NotiModel.LoginDatas = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                NotiModel.LoginDatas = Enumerable.Empty<LoginData>();
            }
            NotiModel.LoginDatas = NotiModel.LoginDatas ?? Enumerable.Empty<LoginData>();

            
            
            Dictionary<int, string> encryptedIdsPay = new Dictionary<int, string>();
            foreach (var x in NotiModel.PaypalAccounts)
            {
                encryptedIdsPay.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsPay = encryptedIdsPay;

            Dictionary<int, string> encryptedIdsLogin = new Dictionary<int, string>();
            foreach (var x in NotiModel.LoginDatas)
            {
                encryptedIdsLogin.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsCard = encryptedIdsLogin;


            return View("Views/Wallet/Notification.cshtml", NotiModel);
            return View("Views/Wallet/Notification.cshtml");
        }
        [Route("Pwned")]
        public async Task<IActionResult> Pwned()
        {
            /*
            IEnumerable<LoginData> userLoginDatas;
            IEnumerable<PaypalAccount> paypalAccounts;
            IEnumerable<Compromised>  compromiseds;
            List<string>
            try
            {
                userLoginDatas = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                userLoginDatas = Enumerable.Empty<LoginData>();
            }
            userLoginDatas = userLoginDatas ?? Enumerable.Empty<LoginData>();

            foreach (var x in userLoginDatas)
            {
                if(x.Compromised==1)
                {
                    compromiseds.
                }
            }

            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userLoginDatas)
            {
                
            }
            ViewBag.EncryptedIds = encryptedIds;
            */
            return PartialView("Views/Wallet/Pwned.cshtml");
            
        }
        [Route("Expired")]
        public IActionResult Expired()
        {
            return PartialView("Views/Wallet/Expired.cshtml");
        }
    }
}
