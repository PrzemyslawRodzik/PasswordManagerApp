using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;


using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Handlers;


using System.Collections.Generic;
using PasswordManagerApp.Services;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Cache;
using Microsoft.Extensions.Configuration;
using System.Threading;
using cloudscribe.Pagination.Models;

namespace PasswordManagerApp.Controllers
{
    [Authorize]
    public class LoginDataController : Controller
    {
        
        public DataProtectionHelper dataProtectionHelper;
        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _config;
        private readonly NotificationService _notify;

        public LoginDataController(IDataProtectionProvider provider,ApiService apiService, EncryptionService encryptionService, ICacheService cacheService,IConfiguration config, NotificationService notify)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
            _notify = notify;
        }   
        
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        [Route("logindatas")]
        public async Task<IActionResult> List(string searchString,int pageNumber=1, int pageSize=5)
        {
            IEnumerable<LoginData> loginDatas;

            try
            {
                loginDatas = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                loginDatas = Enumerable.Empty<LoginData>();
            }
            loginDatas = loginDatas ?? Enumerable.Empty<LoginData>();

            

            if (!String.IsNullOrEmpty(searchString))
                loginDatas = ViewHelper.FilterResult<LoginData>(loginDatas,searchString);
                
            

            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in loginDatas)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), _config["QueryStringsEncryptions"]));

            }
            ViewBag.EncryptedIds = encryptedIds;
            
            return View("Views/Wallet/ListItem.cshtml", ViewHelper.PaginateResult<LoginData>(loginDatas,pageSize,pageNumber));
        }

        

        [Route("loginDataDetails")]
        public async Task<IActionResult> GetLoginDataById(string encrypted_id)
        {
            var loginDataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var logins = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId)));
            var loginData = logins.FirstOrDefault(x => x.Id == loginDataId);
            var decryptedLoginData = DecryptModel(loginData);
            OnDataView(new LoginData { Name = loginData.Name, Password = decryptedLoginData.Password, UserId = loginData.UserId });
            return PartialView("Views/Forms/DetailsLoginData.cshtml", decryptedLoginData);
            
        }
        private void OnDataView(IPasswordModel e)
        {
            _notify.DataViewEvent(e);
        }
        public async Task<IActionResult> AddOrEditLoginData(string encrypted_id)
        {
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", new LoginDataViewModel());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
                var loginDatas = await _cacheService.GetOrCreateCachedResponse<LoginData>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<LoginData>(Int32.Parse(AuthUserId)));
                var loginData = loginDatas.FirstOrDefault(x => x.Id == decrypted_id);
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", DecryptModel(loginData));
            }

        }

        private LoginDataViewModel DecryptModel(LoginData model)
        {
            
           return new LoginDataViewModel
            {
                Login = model.Login,
                Email = model.Email,
                Website = model.Website,
                ModifiedDate = model.ModifiedDate,
                Password = _encryptionService.Decrypt(AuthUserId, model.Password),
                Name = model.Name,

            };
            
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditLoginData(LoginDataViewModel loginDataView, string Id){
            if(!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", loginDataView);
            var loginData = new LoginData
            {
                Id = 0,
                Name = loginDataView.Name,
                Login = loginDataView.Login,
                Email = loginDataView.Email,
                Website = loginDataView.Website,
                Compromised = PwnedPasswords.IsPasswordPwnedAsync(loginDataView.Password, 
                                                                new CancellationToken(), null).Result==-1 ? 0 : 1,
                Password = _encryptionService.Encrypt(AuthUserId, 
                                                    loginDataView.Password)   
            };
            loginData.UserId = Int32.Parse(AuthUserId);
            if (!Id.Equals("0"))
                 loginData.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, _config["QueryStringsEncryptions"]));
            if (loginData.Id == 0)
              await _apiService.CreateUpdateData<LoginData>(loginData);
            else
              await _apiService.CreateUpdateData<LoginData>(loginData, loginData.Id);
            ClearCache();
            OnDataEdit(loginData);
             return RedirectToAction("List");
            
        }
        private void ClearCache()
        {
            _cacheService.ClearCache(CacheKeys.LoginData + AuthUserId);
            _cacheService.ClearCache(CacheKeys.LoginDataBreached + AuthUserId);
            _cacheService.ClearCache(CacheKeys.LoginDataExpired + AuthUserId);
        }
        private async void OnDataEdit(ICompromisedModel e)
        {   await Task.Delay(2000);
            _notify.DataEditEvent(e);
        }
        [HttpPost]
        [Route("logindata/deletelogindata")]
        public IActionResult DeleteLoginData(string encrypted_id)
        {
            var dataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var result = _apiService.DeleteData<LoginData>(dataId).Result;
            ClearCache();
            return RedirectToAction("List");
        }
        
    }
}