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

namespace PasswordManagerApp.Controllers
{



    [Authorize]
    public class LoginDataController : Controller
    {
        
        public DataProtectionHelper dataProtectionHelper;
        private readonly ApiService _apiService;

        public LoginDataController(IDataProtectionProvider provider,ApiService apiService)
        {
            
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            
        }
        



        


        [Route("logindatas")]
        public async Task<IActionResult> List()
        {

            var userId = HttpContext.User.Identity.Name;


            IEnumerable<LoginData> loginDatas;

            try
            {
                loginDatas = await _apiService.GetAllUserData<LoginData>(Int32.Parse(userId));
            }
            catch (HttpRequestException)
            {
                loginDatas = Enumerable.Empty<LoginData>();
            }
            loginDatas = loginDatas ?? Enumerable.Empty<LoginData>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in loginDatas)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListItem.cshtml", loginDatas);
        }

       
        public async Task<IActionResult> AddOrEditLoginData(string Encrypted_id)
        {
            if (Encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", new LoginDataViewModel());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(Encrypted_id, "QueryStringsEncryptions"));

                
                var loginData = await _apiService.GetDataById<LoginData>(decrypted_id);
                var loginDataView = new LoginDataViewModel
                {
                    Login = loginData.Login,
                    Email = loginData.Email,
                    Website = loginData.Website,
                    Password = loginData.Password,
                    Name = loginData.Name,

                };

                ViewBag.Id = Encrypted_id;
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", loginDataView);
            }

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditLoginData(LoginDataViewModel loginDataView, string Id)
        {
            if(!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditLoginData.cshtml", loginDataView);

            var loginData = new LoginData
            {
                Id = 0,
                Login = loginDataView.Login,
                Email = loginDataView.Email,
                Website = loginDataView.Website,
                Password = loginDataView.Password,
                Name = loginDataView.Name,
            };

            loginData.UserId = Int32.Parse(HttpContext.User.Identity.Name);
            
           
            if (!Id.Equals("0"))
                 loginData.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

            if (loginData.Id == 0)
              {
                    await _apiService.CreateUpdateData<LoginData>(loginData);

                    return RedirectToAction("List");
             }
            else
              {
                    await _apiService.CreateUpdateData<LoginData>(loginData, loginData.Id);
                    return RedirectToAction("List");

              }

            
            
        }
        [HttpPost]
        [Route("DeleteLoginData")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLoginData(string encrypted_id)
        {

            var dataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));

            await _apiService.DeleteData<LoginData>(dataId);

            return RedirectToAction(nameof(Index));
        }
    }
}