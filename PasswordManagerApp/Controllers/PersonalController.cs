using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Routing;
using PasswordManagerApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;


using PasswordManagerApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Handlers;


using PasswordManagerApp.Services;
using Microsoft.Extensions.Configuration;

using System.Net.Http;
using Newtonsoft.Json;
using PasswordManagerApp.Cache;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// GET: /<controller>/

namespace PasswordManagerApp.Controllers
{
    
    public class PersonalController : Controller
    {
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        public DataProtectionHelper dataProtectionHelper;
        public PersonalController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService, ICacheService cacheService)
        {
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
        }

       

        [Route("Personal")]
        public async Task<IActionResult> Personal()
        {
            IEnumerable<PersonalInfo> userPersonal;

            try
            {
                userPersonal = await _cacheService.GetOrCreateCachedResponse<PersonalInfo>(CacheKeys.PersonalInfo + AuthUserId, () => _apiService.GetAllUserData<PersonalInfo>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                userPersonal = Enumerable.Empty<PersonalInfo>();
            }
            userPersonal = userPersonal ?? Enumerable.Empty<PersonalInfo>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userPersonal)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListPersonal.cshtml", userPersonal);

                }

       
        [Route("AddOrEditPersonal")]
        public async Task<IActionResult> AddOrEditPersonal(string? encrypted_id)
        {
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditPersonal.cshtml", new PersonalInfo());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));



                var personalinfos = await _cacheService.GetOrCreateCachedResponse<Note>(CacheKeys.Note + AuthUserId, () => _apiService.GetAllUserData<Note>(Int32.Parse(AuthUserId)));
                var personalinfo = personalinfos.FirstOrDefault(x => x.Id == decrypted_id);





                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditPersonal.cshtml", personalinfo);
            }

        }
        [HttpPost]
        [Route("AddOrEditPersonal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditPersonal([Bind("Name,Second_name,Last_name,Date_of_birth")] PersonalInfo note, string Id)
        {
            note.UserId = Int32.Parse(AuthUserId);


            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditPersonal.cshtml", note);


           
            if (!Id.Equals("0"))
                note.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

            if (note.Id == 0)
            {
                await _apiService.CreateUpdateData<PersonalInfo>(note);
            }
            else
            {
                await _apiService.CreateUpdateData<PersonalInfo>(note, note.Id);
            }

            _cacheService.ClearCache(CacheKeys.PersonalInfo + AuthUserId);

            return RedirectToAction("Personal");

        }




        /* [Route("AdresList")]
         public IActionResult AdresList()
         {
             var user = _unitOfWork.Users.Find<User>(int.Parse(HttpContext.User.Identity.Name));
             var tempPersonal = _unitOfWork.Wallet.FindByCondition<PersonalInfo>(x => x.User == user);

             var AdresInfo = _unitOfWork.Wallet.FindByCondition<Address>(x => x.PersonalInfo == tempPersonal);

             Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
             foreach (var x in AdresInfo)
             {
                 encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
                 //  x.Encrypted_Id = dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions");
             }
             ViewBag.EncryptedIds = encryptedIds;
             return View("Views/Wallet/ListAdres.cshtml", AdresInfo);
         //    var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));
          //   var AdresInfo = _unitOfWork.Wallet.FindByCondition<Address>(x => x.User == user);
             //
          //   return View("Views/Wallet/ListAdres.cshtml", AdresInfo);
         }*/
    }
}
