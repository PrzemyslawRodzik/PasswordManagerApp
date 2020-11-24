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
        public DataProtectionHelper dataProtectionHelper;
        public AddressController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
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


        /*
        public async  Task<ActionResult> AddressList()
        {

            //   var user = _unitOfWork.Users.Find<User>(int.Parse(HttpContext.User.Identity.Name));//
            //  var tempPersonal = _unitOfWork.Wallet.FindByCondition<PersonalInfo>(x => x.User == user).First();  //.First(
            //    var AdresInfo = _unitOfWork.Wallet.FindByCondition<Address>(x => x.PersonalInfoId == tempPersonal.Id);

            var userId = HttpContext.User.Identity.Name;
            IEnumerable<Address> userAddressess;
          //  userAddressess = await _apiService.GetAllUserData<Address>(Int32.Parse(userId));
           // var PersonalId=PersonalInfo.Referenc
           

            try
            {
                userAddressess = await _apiService.GetAllUserData<Address>(Int32.Parse(userId));
            }
            catch (HttpRequestException)
            {
                userAddressess = Enumerable.Empty<Address>();
            }
            userAddressess = userAddressess ?? Enumerable.Empty<Address>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userAddressess)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListAddress.cshtml", userAddressess);
        }

        */

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
               Country=model.Country,
              City=model.City
            };
        }

        [HttpPost]
        [Route("AddOrEditAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditAddress([Bind("address_name,street,zip_code,city,number")] Address address, string Id)
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
        [Route("DeleteAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(string encrypted_id)
        {


            var AddressId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));


            await _apiService.DeleteData<Note>(AddressId);
            _cacheService.ClearCache(CacheKeys.Address + AuthUserId);

            return RedirectToAction("AddressList");
        }

        /*
        // GET: Address/Create
        public async Task<IActionResult> AddOrEditAddress(string encrypted_id)
        {

            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditAddress.cshtml", new Address());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));

               // var adresik = _unitOfWork.Context.Addresses.Find(decrypted_id);
                var adresik =  await _apiService.GetDataById<Address>(decrypted_id);
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditAddress.cshtml", adresik);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditAddress([Bind("Address_name,Street,Zip_code,City,Country")] Address address, string Id)
        {
            int temp_id = Int32.Parse(HttpContext.User.Identity.Name);
            address.PersonalInfoId = temp_id;

            //  note.Id = Int32.Parse(dataProtectionHelper.Decrypt(ViewBag.Id, "QueryStringsEncryptions"));
            if (ModelState.IsValid)
            {

                if (!Id.Equals("0"))
                    address.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

                if (address.Id == 0)
                {
                    _unitOfWork.Context.Addresses.Add(address);
                    await _unitOfWork.Context.SaveChangesAsync();
                    return RedirectToAction("AddressList");
                }
                else
                {
                    _unitOfWork.Context.Addresses.Update(address);
                    await _unitOfWork.Context.SaveChangesAsync();
                    return RedirectToAction("AddressList");

                }

            }

            return RedirectToAction("AddressList");
        }

        [HttpPost]
        [Route("DeleteAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(string encrypted_id)
        {

            var addressId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
            var address = await _unitOfWork.Context.Addresses.FindAsync(addressId);
            _unitOfWork.Context.Addresses.Remove(address);
            await _unitOfWork.Context.SaveChangesAsync();
            return RedirectToAction(nameof(AddressList));

        }*/
    }
}