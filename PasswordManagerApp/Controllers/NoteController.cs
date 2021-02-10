using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Linq;
using PasswordManagerApp.Cache;
using Microsoft.AspNetCore.Http;

namespace PasswordManagerApp.Controllers
{

    [Authorize]
    public class NoteController : Controller
    { 
        
        private readonly ApiService _apiService;
        private readonly EncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        public DataProtectionHelper dataProtectionHelper;
        public NoteController(IDataProtectionProvider provider, ApiService apiService, EncryptionService encryptionService,ICacheService cacheService)
        {
            
            
            
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
            _encryptionService = encryptionService;
            _cacheService = cacheService;
        }
        
        public string AuthUserId { get { return HttpContext.User.Identity.Name; } }

        [Route("notes")]
        public async Task<IActionResult> Note()
        {
            IEnumerable<Note> userNotes;
            
            try
            {
                userNotes = await _cacheService.GetOrCreateCachedResponse<Note>(CacheKeys.Note+ AuthUserId, () => _apiService.GetAllUserData<Note>(Int32.Parse(AuthUserId))); 
                    
            }
            catch (HttpRequestException)
            {
                userNotes = Enumerable.Empty<Note>();
            }
            userNotes = userNotes ?? Enumerable.Empty<Note>();

                Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
               foreach(var x in userNotes)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
              
            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListNote.cshtml", userNotes);
        }
        
        [Route("noteDetails")]
        
        public async Task<IActionResult> GetNoteById(string encrypted_id)
        {
             
            
             var noteId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
            
            
            var notes = await _cacheService.GetOrCreateCachedResponse<Note>(CacheKeys.Note + AuthUserId, () => _apiService.GetAllUserData<Note>(Int32.Parse(AuthUserId)));
            var note = notes.FirstOrDefault(x => x.Id == noteId);
            return PartialView("Views/Forms/DetailsNote.cshtml", DecryptModel(note));
        }


        
        [Route("AddOrEditNote")]
        public async Task<IActionResult> AddOrEditNote(string? encrypted_id)
        {
            if(encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditNote.cshtml", new Note());
            }
            else
            {
                int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
                
                
                
                var notes = await _cacheService.GetOrCreateCachedResponse<Note>(CacheKeys.Note + AuthUserId, () => _apiService.GetAllUserData<Note>(Int32.Parse(AuthUserId)));
                var note = notes.FirstOrDefault(x => x.Id == decrypted_id);

                

                
                
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditNote.cshtml", DecryptModel(note));
            }

        }

        private Note DecryptModel(Note model)
        {
            return new Note
            {
                Name = model.Name,
                Details = _encryptionService.Decrypt(AuthUserId, model.Details)
            };
        }

        [HttpPost]
        [Route("AddOrEditNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditNote([Bind("Name,Details")] Note note, string Id)
        {
            note.UserId = Int32.Parse(AuthUserId);
            
            
            if (!ModelState.IsValid)
                return PartialView("Views/Forms/AddOrEditNote.cshtml", note);


            note.Details = _encryptionService.Encrypt(AuthUserId,note.Details);
            if (!Id.Equals("0"))
               note.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

                if (note.Id==0)
                {
                    await _apiService.CreateUpdateData<Note>(note);
                }
                else
                {
                    await _apiService.CreateUpdateData<Note>(note,note.Id);
                }
                
                _cacheService.ClearCache(CacheKeys.Note + AuthUserId);

                return RedirectToAction("Note");

        }
        [HttpPost]
        [Route("deletenote")]
       
        public IActionResult DeleteNote(string encrypted_id)
        {
             
            
             var noteId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
           

            var result = _apiService.DeleteData<Note>(noteId).Result;
            _cacheService.ClearCache(CacheKeys.Note + AuthUserId);

            return RedirectToAction("Note");
        }

        
        [Route("decryptData")]
        public string DecryptField(string enc_data) =>  _encryptionService.Decrypt(AuthUserId, enc_data);










    }
}



