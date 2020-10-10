using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace PasswordManagerApp.Controllers
{
    
    [Authorize]
    public class NoteController : Controller
    { 
        
        private readonly ApiService _apiService;
        public DataProtectionHelper dataProtectionHelper;
        public NoteController(IDataProtectionProvider provider, ApiService apiService)
        {
            
            
            
            dataProtectionHelper = new DataProtectionHelper(provider);
            _apiService = apiService;
        }

        [Route("notes")]
        public async Task<IActionResult> Note()
        {
            


             var userId = HttpContext.User.Identity.Name;
            
            
            IEnumerable<Note> userNotes;
            
            try
            {
                userNotes = await _apiService.GetAllUserData<Note>(Int32.Parse( userId));
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
                
                
                var notatka = await _apiService.GetDataById<Note>(decrypted_id);
               
                ViewBag.Id = encrypted_id;
                return PartialView("Views/Forms/AddOrEditNote.cshtml", notatka);
            }

        }

        [HttpPost]
        [Route("AddOrEditNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditNote([Bind("Name,Details")] Note note, string Id)
        {
             note.UserId = Int32.Parse(HttpContext.User.Identity.Name);
            
            
            if (ModelState.IsValid)
            {
                if (!Id.Equals("0"))
                    note.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

                if (note.Id==0)
                {
                    
                    await _apiService.CreateUpdateData<Note>(note);
                    

                    return RedirectToAction("Note");
                }
                else
                {
                    await _apiService.CreateUpdateData<Note>(note,note.Id);
                    return RedirectToAction("Note");

                }

            }

            return PartialView("Views/Forms/AddOrEditNote.cshtml", note);
        }
        [HttpPost]
        [Route("DeleteNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNote(string encrypted_id)
        {
             
            
             var noteId = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
           

            await _apiService.DeleteData<Note>(noteId);


            return RedirectToAction("Note");
        }




    }
}



