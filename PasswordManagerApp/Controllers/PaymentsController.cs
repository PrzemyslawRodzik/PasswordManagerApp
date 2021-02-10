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
    public class PaymentsController : Controller
    {
        public DataProtectionHelper dataProtectionHelper;
        private readonly ApiService _apiService;
        private readonly ICacheService _cacheService;
        private readonly EncryptionService _encryptionService;
        private readonly IConfiguration _config;
        public PaymentsController(IDataProtectionProvider provider, ApiService apiService,  EncryptionService encryptionService, ICacheService cacheService, IConfiguration config)
        {

            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
            _config = config;
        }
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        
        [Route("Payments")]
        public async Task<IActionResult> List(string searchString, int payNumber = 1, int paySize = 5,int cardNumber=1,int cardSize=5)
        {
            
            var userId = HttpContext.User.Identity.Name;
            PaymentsViewModel PaymentsModel = new PaymentsViewModel();
             try
            {
                PaymentsModel.CreditCards = await _cacheService.GetOrCreateCachedResponse<CreditCard>(CacheKeys.CreditCard + AuthUserId, () => _apiService.GetAllUserData<CreditCard>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                PaymentsModel.CreditCards = Enumerable.Empty<CreditCard>();
            }
            PaymentsModel.CreditCards = PaymentsModel.CreditCards ?? Enumerable.Empty<CreditCard>();

            try
            {
                PaymentsModel.PaypalAccounts = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccount + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                PaymentsModel.PaypalAccounts = Enumerable.Empty<PaypalAccount>();
            }
            PaymentsModel.PaypalAccounts = PaymentsModel.PaypalAccounts ?? Enumerable.Empty<PaypalAccount>();

           

            if (!String.IsNullOrEmpty(searchString))
            {
              //  PaymentsModel.CreditCards = ViewHelper.FilterResult<CreditCard>(PaymentsModel.CreditCards, searchString);


            }

           

            Dictionary<int, string> encryptedIdsPay = new Dictionary<int, string>();
            foreach (var x in PaymentsModel.PaypalAccounts)
            {
                encryptedIdsPay.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsPay = encryptedIdsPay;

            Dictionary<int, string> encryptedIdsCard = new Dictionary<int, string>();
            foreach (var x in PaymentsModel.CreditCards)
            {
                encryptedIdsCard.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsCard = encryptedIdsCard;


            return View("Views/Wallet/UltraPay.cshtml", PaymentsModel);


        }

        [Route("AddOrEditPayPal")]
        public async Task<IActionResult> AddOrEditPayPal(string encrypted_id)
        {
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditPayPal.cshtml", new PaypalAccount());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));

                var paypals = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.CreditCard + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));
                var paypal = paypals.FirstOrDefault(x => x.Id == decrypted_id);
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditPayPal.cshtml", DecryptModelPay(paypal));
            }
        }
       
        
        [HttpPost]
        [Route("AddOrEditPayPal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditPayPal([Bind("Name,Email,Password")] PaypalAccount paypal, string Id)
        {
            paypal.UserId = Int32.Parse(AuthUserId);
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditPayPal.cshtml", paypal);
            paypal.Compromised = PwnedPasswords.IsPasswordPwnedAsync(paypal.Password, new CancellationToken(), null).Result == -1 ? 0 : 1;
            paypal.Password = _encryptionService.Encrypt(AuthUserId,paypal.Password);
            

            if (!Id.Equals("0"))
                paypal.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

            if (paypal.Id == 0)
            {
                await _apiService.CreateUpdateData<PaypalAccount>(paypal);
            }
            else
            {
                await _apiService.CreateUpdateData<PaypalAccount>(paypal, paypal.Id);
            }
            ClearCache();
                   
                     
            return RedirectToAction("List");
        }
        private void ClearCache()
        {
            _cacheService.ClearCache(CacheKeys.PaypalAccount + AuthUserId);
            _cacheService.ClearCache(CacheKeys.PaypalAccountBreached + AuthUserId);
            _cacheService.ClearCache(CacheKeys.PaypalAccountExpired + AuthUserId);
        }

        [Route("AddOrEditCreditCard")]
        public async Task<IActionResult> AddOrEditCreditCard(string encrypted_id)
        {
            
            if (encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditCreditCard.cshtml", new CreditCard());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
            var cards = await _cacheService.GetOrCreateCachedResponse<CreditCard>(CacheKeys.CreditCard + AuthUserId, () => _apiService.GetAllUserData<CreditCard>(Int32.Parse(AuthUserId)));

                var card = cards.FirstOrDefault(x => x.Id == decrypted_id);
               
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditCreditCard.cshtml", DecryptModelCard(card));
            }
        }

        [HttpPost]
        [Route("AddOrEditCreditCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditCreditCard([Bind("Name,CardHolderName, CardNumber,SecurityCode, ExpirationDate")] CreditCard card, string Id)
        {
      
            card.UserId = Int32.Parse(AuthUserId);
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEdiCreditCard.cshtml", card);

            card.SecurityCode = _encryptionService.Encrypt(AuthUserId, card.SecurityCode);
            card.CardNumber= _encryptionService.Encrypt(AuthUserId, card.CardNumber);
            

            if (!Id.Equals("0"))
                card.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));


            if (card.Id == 0)
            {
                await _apiService.CreateUpdateData<CreditCard>(card);
            }
            else
            {
                await _apiService.CreateUpdateData<CreditCard>(card, card.Id);
            }

            _cacheService.ClearCache(CacheKeys.CreditCard + AuthUserId);
            

          
            return RedirectToAction("List");
        }

        [HttpPost]
        [Route("payments/deletepaypal")]
        public IActionResult DeletePaypal(string encrypted_id)
        {
            var dataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var result = _apiService.DeleteData<PaypalAccount>(dataId).Result;
            ClearCache();
            return RedirectToAction("List");
        }
        [HttpPost]
        [Route("payments/deletecard")]
        public IActionResult DeleteCard(string encrypted_id)
        {
            var dataId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var result = _apiService.DeleteData<CreditCard>(dataId).Result;
            _cacheService.ClearCache(CacheKeys.CreditCard + AuthUserId);
            return RedirectToAction("List");
        }

       

        private CreditCard DecryptModelCard(CreditCard model)
        {
            return new CreditCard
            {
                Name = model.Name,
                CardHolderName = model.CardHolderName,
                CardNumber = _encryptionService.Decrypt(AuthUserId, model.CardNumber),
                SecurityCode = _encryptionService.Decrypt(AuthUserId, model.SecurityCode),
                ExpirationDate = model.ExpirationDate
            };

        }
        [Route("CreditCardDetails")]
        public async Task<IActionResult> GetCreditCardById(string encrypted_id)
        {
            var cardId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var cards = await _cacheService.GetOrCreateCachedResponse<CreditCard>(CacheKeys.CreditCard + AuthUserId, () => _apiService.GetAllUserData<CreditCard>(Int32.Parse(AuthUserId)));
            var card = cards.FirstOrDefault(x => x.Id == cardId);
            return PartialView("Views/Forms/DetailsCreditCard.cshtml", DecryptModelCard(card));
        }
     




        private PaypalAccount DecryptModelPay(PaypalAccount model)
        {
            return new PaypalAccount
            {   Name = model.Name,
                Email = model.Email,
                Password = _encryptionService.Decrypt(AuthUserId, model.Password)
            };
        }

        [Route("PaypalDetails")]
        public async Task<IActionResult> GetPaypalById(string encrypted_id)
        {
            var paypalId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, _config["QueryStringsEncryptions"]));
            var paypals = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccount + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));
            var paypal = paypals.FirstOrDefault(x => x.Id == paypalId);
            return PartialView("Views/Forms/DetailsPayPal.cshtml", DecryptModelPay(paypal));
        }




    }
}