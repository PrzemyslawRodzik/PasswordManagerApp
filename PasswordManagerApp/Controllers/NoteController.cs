using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Microsoft.Extensions.Configuration;
using PasswordManagerApp.Handlers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;

namespace PasswordManagerApp.Controllers
{
    [Authorize]
    
    public class NoteController : Controller
    { 
        private readonly IUnitOfWork _unitOfWork;
        public DataProtectionHelper dataProtectionHelper;
        public NoteController(IUnitOfWork unitOfWork, IDataProtectionProvider provider)
        {
            
            _unitOfWork = unitOfWork;
            
            dataProtectionHelper = new DataProtectionHelper(provider);
        }

        [Route("notes")]
        public IActionResult Note()
        {

            var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));
            var NoteInfo1 = _unitOfWork.Wallet.FindByCondition<Note>(x => x.User == user);
            Dictionary<int, string> encryptedIds = new Dictionary<int, string>();
               foreach(var x in NoteInfo1)
            {
                encryptedIds.Add(x.Id, dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions"));
              //  x.Encrypted_Id = dataProtectionHelper.Encrypt(x.Id.ToString(), "QueryStringsEncryptions");
            }
            ViewBag.EncryptedIds = encryptedIds;
            return View("Views/Wallet/ListNote.cshtml", NoteInfo1);
        }
       

        [Route("AddOrEditNote")]
        public IActionResult AddOrEditNote(string? encrypted_id)
        {
            if(encrypted_id is null)
            {
                ViewBag.Id = 0;
                return PartialView("Views/Forms/AddOrEditNote.cshtml", new Note());
            }
            else
            {
               int decrypted_id = Int32.Parse(dataProtectionHelper.Decrypt(encrypted_id, "QueryStringsEncryptions"));
                
                var notatka = _unitOfWork.Context.Notes.Find(decrypted_id);
               
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
          //  note.Id = Int32.Parse(dataProtectionHelper.Decrypt(ViewBag.Id, "QueryStringsEncryptions"));
            if (ModelState.IsValid)
            {
                if (!Id.Equals("0"))
                    note.Id = Int32.Parse(dataProtectionHelper.Decrypt(Id, "QueryStringsEncryptions"));

                if (note.Id==0)
                {
                    _unitOfWork.Context.Notes.Add(note);
                    await _unitOfWork.Context.SaveChangesAsync();
                    return RedirectToAction("Note");
                }
                else
                {
                    _unitOfWork.Context.Notes.Update(note);
                    await _unitOfWork.Context.SaveChangesAsync();
                    return RedirectToAction("Note");

                }

            }

            return RedirectToAction("Note");
        }
        

        
        
}
}



