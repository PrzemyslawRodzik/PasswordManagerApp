using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using PasswordManagerApp.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;
using PasswordManagerApp.Cache;
using System.ComponentModel.DataAnnotations;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PasswordManagerApp.Controllers
{
    
    public class PhoneController : Controller
    {
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }


        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _config;
        public DataProtectionHelper dataProtectionHelper;
        public PhoneController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService,IConfiguration config)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
        }
        [Route("PhoneList")]

        public async Task<IActionResult> PhoneList()
        {
            IEnumerable<PhoneNumber> userphones;

            try
            {
                userphones = await _cacheService.GetOrCreateCachedResponse<PhoneNumber>(CacheKeys.PhoneNumber + AuthUserId, () => _apiService.GetAllUserData<PhoneNumber>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                userphones = Enumerable.Empty<PhoneNumber>();
            }
            userphones = userphones ?? Enumerable.Empty<PhoneNumber>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userphones)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), _config["QueryStringsEncryptions"]));

            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListPhone.cshtml", userphones);

        }
       
        [Route("AddOrEditPhone")]
        public async Task<IActionResult> AddOrEditPhone(string encrypted_id)
        {
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditPhone.cshtml", new PhoneNumber());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));

                var phones = await _cacheService.GetOrCreateCachedResponse<PhoneNumber>(CacheKeys.PhoneNumber + AuthUserId, () => _apiService.GetAllUserData<PhoneNumber>(Int32.Parse(AuthUserId)));
                var phone = phones.FirstOrDefault(x => x.Id == decrypted_id);


                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditPhone.cshtml", phone);
            }

        }
        

        [HttpPost]
        [Route("AddOrEditPhone")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditPhone([Bind("NickName,TelNumber,Type")] PhoneNumber phone, string Id)
        {
            int temp_id = Int32.Parse(HttpContext.User.Identity.Name);
            phone.UserId = temp_id;

            
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditPhone.cshtml", phone);

            if (!Id.Equals("0"))
                    phone.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

                if (phone.Id == 0)
                {
                    await _apiService.CreateUpdateData<PhoneNumber>(phone);
                }
                else
                {
                    await _apiService.CreateUpdateData<PhoneNumber>(phone, phone.Id);

                }

            _cacheService.ClearCache(CacheKeys.PhoneNumber + AuthUserId);

            return RedirectToAction("PhoneList");
        }


        [HttpPost]
        [Route("phone/deletephone")]
        public IActionResult DeletePhone(string encrypted_id)
        {
            var phoneId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var result = _apiService.DeleteData<PhoneNumber>(phoneId).Result;
            _cacheService.ClearCache(CacheKeys.PhoneNumber + AuthUserId);
            return RedirectToAction("PhoneList");
        }
        [Route("GetPhoneById")]
        public async Task<IActionResult> GetPhoneById(string encrypted_id)
        {
            var phoneId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var phones = await _cacheService.GetOrCreateCachedResponse<PhoneNumber>(CacheKeys.PhoneNumber + AuthUserId, () => _apiService.GetAllUserData<PhoneNumber>(Int32.Parse(AuthUserId)));
            var phone = phones.FirstOrDefault(x => x.Id == phoneId);
            return PartialView("Views/Forms/DetailsPhone.cshtml", phone);
        }


    }
}

