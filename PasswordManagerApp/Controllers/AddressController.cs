using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


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

namespace PasswordManagerApp.Controllers
{

    public class AddressController : Controller
    {
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }


        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _config;
        public DataProtectionHelper dataProtectionHelper;
        public AddressController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService, IConfiguration config)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
        }

        [Route("AddressList")]

        public async Task<IActionResult> AddressList()
        {
            IEnumerable<Address> useradres;

            try
            {
                useradres = await _cacheService.GetOrCreateCachedResponse<Address>(CacheKeys.Address + AuthUserId, () => _apiService.GetAllUserData<Address>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                useradres = Enumerable.Empty<Address>();
            }
            useradres = useradres ?? Enumerable.Empty<Address>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in useradres)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListAddress.cshtml", useradres);

        }


       

        [Route("AddOrEditAddress")]
        public async Task<IActionResult> AddOrEditAddress(string? encrypted_id)
        {
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditAddress.cshtml", new Address());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));



                var adressess = await _cacheService.GetOrCreateCachedResponse<Address>(CacheKeys.Address + AuthUserId, () => _apiService.GetAllUserData<Address>(Int32.Parse(AuthUserId)));
                var address = adressess.FirstOrDefault(x => x.Id == decrypted_id);





                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditAddress.cshtml", DecryptModel(address));
            }

        }

        private Address DecryptModel(Address model)
        {
            return new Address
            {
                AddressName = model.AddressName,
               Street=model.Street,
               ZipCode=model.ZipCode,
               StreetNumber=model.StreetNumber,
              City=model.City
            };
        }

        [HttpPost]
        [Route("AddOrEditAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditAddress([Bind("AddressName,Street,StreetNumber,ZipCode,City")] Address address, string Id)
        {
           
            address.UserId = Int32.Parse(AuthUserId);
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditAddress.cshtml", address);

           
            
            if (!Id.Equals("0"))
                address.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

            if (address.Id == 0)
            {
                await _apiService.CreateUpdateData<Address>(address);
            }
            else
            {
                await _apiService.CreateUpdateData<Address>(address, address.Id);
            }

            _cacheService.ClearCache(CacheKeys.Address + AuthUserId);

            return RedirectToAction("AddressList");
        }
       
        [HttpPost]
        [Route("address/deleteaddress")]
        public IActionResult DeleteAddress(string encrypted_id)
        {
            var dataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var result = _apiService.DeleteData<Address>(dataId).Result;
            _cacheService.ClearCache(CacheKeys.Address + AuthUserId);
            return RedirectToAction("AddressList");
        }

        [Route("GetAddressById")]
        public async Task<IActionResult> GetAddressById(string encrypted_id)
        {
            var addressId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var addresses = await _cacheService.GetOrCreateCachedResponse<Address>(CacheKeys.Address + AuthUserId, () => _apiService.GetAllUserData<Address>(Int32.Parse(AuthUserId)));
            var address = addresses.FirstOrDefault(x => x.Id == addressId);
            return PartialView("Views/Forms/DetailsAddress.cshtml", DecryptModel(address));
        }



    }
}