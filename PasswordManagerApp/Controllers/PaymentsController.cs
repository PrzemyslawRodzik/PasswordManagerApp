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

using System.Security.Claims;
using PasswordManagerApp.Cache;


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

        [Route("PayPals")]
        public async Task<IActionResult> ListPayPal()
        {
            IEnumerable<PaypalAccount> userPayPals;

            try
            {
                userPayPals = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.PaypalAccount + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                userPayPals = Enumerable.Empty<PaypalAccount>();
            }
            userPayPals = userPayPals ?? Enumerable.Empty<PaypalAccount>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userPayPals)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsPayPal = encryptedIds;
            return PartialView("Views/Wallet/ListPayPal.cshtml", userPayPals);
        }
        [Route("CreditCards")]
        public async Task<IActionResult>ListCreditCard()
        {
            IEnumerable<CreditCard> userCards;

            try
            {
                userCards = await _cacheService.GetOrCreateCachedResponse<CreditCard>(CacheKeys.CreditCard + AuthUserId, () => _apiService.GetAllUserData<CreditCard>(Int32.Parse(AuthUserId)));

            }
            catch (HttpRequestException)
            {
                userCards = Enumerable.Empty<CreditCard>();
            }
            userCards = userCards ?? Enumerable.Empty<CreditCard>();


            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            foreach (var x in userCards)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));

            }
            ViewBag.EncryptedIdsCreditCard = encryptedIds;
            return PartialView("Views/Wallet/ListCreditCard.cshtml", userCards);
        }
        [Route("PaymentsList")]
        public async Task<IActionResult> ListPayments()
        {
            return View("Views/Wallet/ListPayments.cshtml");
        }

        [Route("Payments")]
        public async Task<IActionResult> List(string searchString, int payNumber = 1, int paySize = 5,int cardNumber=1,int cardSize=5)
        {
            
            var userId = HttpContext.User.Identity.Name;
            PaymentsViewModel PaymentsModel = new PaymentsViewModel();
             try
            {
                PaymentsModel.CreditCards = await _cacheService.GetOrCreateCachedResponse<CreditCard>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<CreditCard>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                PaymentsModel.CreditCards = Enumerable.Empty<CreditCard>();
            }
            PaymentsModel.CreditCards = PaymentsModel.CreditCards ?? Enumerable.Empty<CreditCard>();

            try
            {
                PaymentsModel.PaypalAccounts = await _cacheService.GetOrCreateCachedResponse<PaypalAccount>(CacheKeys.LoginData + AuthUserId, () => _apiService.GetAllUserData<PaypalAccount>(Int32.Parse(AuthUserId)));
            }
            catch (HttpRequestException)
            {
                PaymentsModel.PaypalAccounts = Enumerable.Empty<PaypalAccount>();
            }
            PaymentsModel.PaypalAccounts = PaymentsModel.PaypalAccounts ?? Enumerable.Empty<PaypalAccount>();

            /*   IEnumerable<PaypallAcount> userPayPals;
               IEnumerable<CreditCard> userCards;
               //   await _apiService.GetAllUserData<Note>(Int32.Parse(userId));
               // var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));
               try
               {
                   userPayPals = await _apiService.GetAllUserData<PaypallAcount>(Int32.Parse(userId));
           //    userCards = await _apiService.GetAllUserData<CreditCard>(Int32.Parse(userId));
               }
               catch (HttpRequestException)
               {
                   userPayPals = Enumerable.Empty<PaypallAcount>();
               }
               userPayPals = userPayPals ?? Enumerable.Empty<PaypallAcount>();

               try
               {
               //    userPayPals = await _apiService.GetAllUserData<PaypallAcount>(Int32.Parse(userId));
                   userCards = await _apiService.GetAllUserData<CreditCard>(Int32.Parse(userId));
               }
               catch (HttpRequestException)
               {
                   userCards = Enumerable.Empty<CreditCard>();
               }*/
            //  userCards = userCards ?? Enumerable.Empty<CreditCard>();

            //  PaymentsModel.PaypalAccounts = _unitOfWork.Wallet.FindByCondition<PaypallAcount>(x => x.User == user);
            // PaymentsModel.CreditCards = _unitOfWork.Wallet.FindByCondition<CreditCard>(x => x.User == user);


            //  var paym = await _apiService.GetDataById<LoginData>(decrypted_id);
            //  PaymentsModel = _unitOfWork.Wallet.FindByCondition<PaymentsViewModel>(x => x.User == user);

            //var PaymentsInfo1 = _unitOfWork.Wallet.FindByCondition<PaypallAcount>(x => x.User == user);
            // var CreditCardInfo = _unitOfWork.Wallet.FindByCondition<CreditCard>(x => x.User == user);

            if (!String.IsNullOrEmpty(searchString))
            {
              //  PaymentsModel.CreditCards = ViewHelper.FilterResult<CreditCard>(PaymentsModel.CreditCards, searchString);


            }

            /*
            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
            if (PaymentsModel.CreditCards.First().Id.ToString() != "null")
            {
                foreach (var x in PaymentsModel.CreditCards)
                {
                    encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
                    //  x.Encrypted_Id = dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions");
                }
            }

            ViewBag.EncryptedIdsPay = encryptedIds;
            if (PaymentsModel.PaypallAcounts.Count() > 0)
            {
                foreach (var x in PaymentsModel.PaypallAcounts)
                {
                    encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
                    //  x.Encrypted_Id = dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions");
                }
            } */

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
                return PartialView("Views/Forms/AddOrEditPayPal.cshtml", paypal);
            }
        }
       
        
        [HttpPost]
        [Route("AddOrEditPayPal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditPayPal([Bind("Email,Password")] PaypalAccount paypal, string Id)
        {
            paypal.UserId = Int32.Parse(AuthUserId);
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditPayPal.cshtml", paypal);

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

            _cacheService.ClearCache(CacheKeys.PaypalAccount + AuthUserId);            
            return RedirectToAction("List");
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
               


                // var card_data = _unitOfWork.Context.CreditCards.Find(decrypted_id);
              //  var card_data = await _apiService.GetDataById<CreditCard>(decrypted_id);

                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditCreditCard.cshtml", card);
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
            card.CardHolderName = _encryptionService.Encrypt(AuthUserId, card.CardHolderName);
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
        [Route("DeletePayPal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayPal(string encrypted_id)
        {

            var payId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
            //  var pay = await _unitOfWork.Context.PaypallAcounts.FindAsync(payId);
            //  _unitOfWork.Context.PaypallAcounts.Remove(pay);
            //  await _unitOfWork.Context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }
        [HttpPost]
        [Route("DeleteCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(string encrypted_id)
        {

            var cardId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
            //  var card = await _unitOfWork.Context.CreditCards.FindAsync(cardId);
            //  _unitOfWork.Context.CreditCards.Remove(card);
            //  await _unitOfWork.Context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> DeletePayPa(string id1)
        {
            int id = Int32.Parse(id1);
            //   var employee = await _unitOfWork.Context.PaypallAcounts.FindAsync(id);
            //   _unitOfWork.Context.PaypallAcounts.Remove(employee);
            //   await _unitOfWork.Context.SaveChangesAsync();
            return RedirectToAction(nameof(List));

        }

        private CreditCard DecryptModelCard(CreditCard model)
        {
            return new CreditCard
            {
                Name = model.Name,
                CardHolderName = _encryptionService.Decrypt(AuthUserId, model.CardHolderName),
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
      /*  public async Task<IActionResult> GetCreditCardById()
        {
            return PartialView("Views/Forms/AddOrEditPayPal.cshtml");
        }*/
        private PaypalAccount DecryptModelPay(PaypalAccount model)
        {
            return new PaypalAccount
            {
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


        [Route("decryptData")]
        public string DecryptField(string enc_data) => _encryptionService.Decrypt(AuthUserId, enc_data);



    }
}