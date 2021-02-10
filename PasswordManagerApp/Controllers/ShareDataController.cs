using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.Cache;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;

namespace PasswordManagerApp.Controllers
{
    public class ShareDataController : Controller
    {
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _config;
        public DataProtectionHelper dataProtectionHelper;
        public ShareDataController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService, IConfiguration config)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
        }
        

        public IActionResult SharedLoginsExportList()
        {
            IEnumerable<ShareLoginModel> logins;

            try
            {
                logins = _cacheService.GetOrCreateCachedResponseSync<ShareLoginModel>(CacheKeys.SharedLoginsExported + AuthUserId, () => _apiService.GetSharedExportLogins(Int32.Parse(AuthUserId)));


            }
            catch (HttpRequestException)
            {
                logins = Enumerable.Empty<ShareLoginModel>();
            }
            logins = logins ?? Enumerable.Empty<ShareLoginModel>();



            return View("Views/Wallet/ListShareData.cshtml", logins);

        }

        public IActionResult SharedLoginsImportList()
        {
            IEnumerable<SharedLoginModel> logins;

            try
            {
                logins =  _cacheService.GetOrCreateCachedResponseSync<SharedLoginModel>(CacheKeys.SharedLoginsImported + AuthUserId, () => _apiService.GetSharedImportLogins(Int32.Parse(AuthUserId)));
                

            }
            catch (HttpRequestException)
            {
                logins = Enumerable.Empty<SharedLoginModel>();
            }
            logins = logins ?? Enumerable.Empty<SharedLoginModel>();
            

            
            return View("Views/Wallet/ListShareDataImport.cshtml", logins);

        }


        
        [Route("GetSharedLoginDataById")]
        public IActionResult GetSharedLoginDataById(string id)
        {
            var intId = Int32.Parse(id);
            var logins = _cacheService.GetOrCreateCachedResponseSync<ShareLoginModel>(CacheKeys.SharedLoginsExported + AuthUserId, () => _apiService.GetSharedExportLogins(Int32.Parse(AuthUserId)));
            var login = logins.FirstOrDefault(x => x.LoginData.Id == intId).LoginData;
            return PartialView("Views/Forms/DetailsLoginData.cshtml", DecryptSharedLogin(login));
        }
        [Route("GetImportedDataById")]
        public IActionResult GetImportedDataById(string id)
        {
            var intId = Int32.Parse(id);
            var logins = _cacheService.GetOrCreateCachedResponseSync<SharedLoginModel>(CacheKeys.SharedLoginsImported + AuthUserId, () => _apiService.GetSharedImportLogins(Int32.Parse(AuthUserId)));
            var login = logins.FirstOrDefault(x => x.LoginData.Id == intId).LoginData;
            return PartialView("Views/Forms/DetailsLoginData.cshtml", DecryptSharedLogin(login));
        }

        private LoginDataViewModel DecryptSharedLogin(LoginData model)
        {
            return new LoginDataViewModel
            {
               Name=model.Name,
               Email = model.Email,
               Login = model.Login,
               Website = model.Website,
               ModifiedDate = model.ModifiedDate,
               Password = _encryptionService.DecryptSharedData(_config["JwtSettings:SecretEncryptionKey"],model.Password)
            };
        }
        






        [HttpPost]
        [Route("share/deletesharedata")]
        public IActionResult DeleteShareData(string id)
        {
            
            var result = _apiService.DeleteSharedData(Int32.Parse(id)).Result;
            _cacheService.ClearCache(CacheKeys.SharedLoginsExported + AuthUserId);
            return RedirectToAction("SharedLoginsExportList");
        }

       


       


    }
}
