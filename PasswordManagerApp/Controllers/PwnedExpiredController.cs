using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Http;
using PasswordManagerApp.Cache;
using System.Threading;

namespace PasswordManagerApp.Controllers
{
    public class PwnedExpiredController : Controller
    {
        public DataProtectionHelper dataProtectionHelper;
        private readonly ApiService _apiService;
        private readonly ICacheService _cacheService;
        private readonly EncryptionService _encryptionService;
        private readonly IConfiguration _config;
        public PwnedExpiredController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService, IConfiguration config)
        {

            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
        }
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }


        [Route("PwnedExpired")]
        public async Task<IActionResult> Index(string searchString, int payNumber = 1, int paySize = 5, int cardNumber = 1, int cardSize = 5)
        {

            var userId = HttpContext.User.Identity.Name;
            PwnedExpiredViewModel pwnedExpiredModel = new PwnedExpiredViewModel();
            try
            {
                pwnedExpiredModel.LoginDatasBreached = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginDataBreached + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId),compromised:1));
                pwnedExpiredModel.LoginDatasExpired = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginDataExpired + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId), expired:1));
            }
            catch (HttpRequestException)
            {
                pwnedExpiredModel.LoginDatasBreached = Enumerable.Empty<LoginData>();
                pwnedExpiredModel.LoginDatasExpired = Enumerable.Empty<LoginData>();
            }
            pwnedExpiredModel.LoginDatasBreached = pwnedExpiredModel.LoginDatasBreached ?? Enumerable.Empty<LoginData>();
            pwnedExpiredModel.LoginDatasExpired = pwnedExpiredModel.LoginDatasExpired ?? Enumerable.Empty<LoginData>();

            try
            {
                pwnedExpiredModel.PaypalAccountsBreached = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccountBreached + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId),compromised:1));
                pwnedExpiredModel.PaypalAccountsExpired = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccountExpired + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId), expired: 1));
            }
            catch (HttpRequestException)
            {
                pwnedExpiredModel.PaypalAccountsBreached = Enumerable.Empty<PaypalAccount>();
                pwnedExpiredModel.PaypalAccountsExpired = Enumerable.Empty<PaypalAccount>();
            }
            pwnedExpiredModel.PaypalAccountsBreached = pwnedExpiredModel.PaypalAccountsBreached ?? Enumerable.Empty<PaypalAccount>();
            pwnedExpiredModel.PaypalAccountsExpired = pwnedExpiredModel.PaypalAccountsExpired ?? Enumerable.Empty<PaypalAccount>();



            if (!String.IsNullOrEmpty(searchString))
            {
                //  PaymentsModel.CreditCards = ViewHelper.FilterResult<CreditCard>(PaymentsModel.CreditCards, searchString);


            }



            Dictionary<int, string> encryptedIdsLogBreached = new Dictionary<int, string>();
            foreach (var x in pwnedExpiredModel.LoginDatasBreached)
            {
                encryptedIdsLogBreached.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.encryptedIdsLogBreached = encryptedIdsLogBreached;
            Dictionary<int, string> encryptedIdsLogExpired = new Dictionary<int, string>();
            foreach (var x in pwnedExpiredModel.LoginDatasExpired)
            {
                encryptedIdsLogExpired.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.encryptedIdsLogExpired = encryptedIdsLogExpired;

            Dictionary<int, string> encryptedIdsPayBreached = new Dictionary<int, string>();
            foreach (var x in pwnedExpiredModel.PaypalAccountsBreached)
            {
                encryptedIdsPayBreached.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.encryptedIdsPayBreached = encryptedIdsPayBreached;
            Dictionary<int, string> encryptedIdsPayExpired = new Dictionary<int, string>();
            foreach (var x in pwnedExpiredModel.PaypalAccountsExpired)
            {
                encryptedIdsPayExpired.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.encryptedIdsPayExpired = encryptedIdsPayExpired;


            return View("Views/Wallet/ListPwnedExpired.cshtml", pwnedExpiredModel);


        }

       


    }
}