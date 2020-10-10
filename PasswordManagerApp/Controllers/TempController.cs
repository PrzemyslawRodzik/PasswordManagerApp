using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PasswordManagerApp.Handlers;

using PasswordManagerApp.Models;
//using PasswordManagerApp.Models;

namespace PasswordManagerApp.Controllers
{   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
      /*  private readonly IUnitOfWork _unitOfWork;
        public TempController()
        {
        }

        [Route("temp")]
        public IActionResult temp()
        {


            var authUserId = Int32.Parse(HttpContext.User.Identity.Name);


            var loginDatas = _unitOfWork.Wallet.GetUnchangedPasswordsForUser(authUserId);
            var loginDatasList = (List<LoginData>)loginDatas;
            if (loginDatas is null)
                return Ok("null byl");
            //MethodSignalR();
            var liczba = loginDatasList.Count;
            string message = $"Liczba wynosi: {liczba} @###########################";

            return Ok(liczba);
        }
        [Route("/ip")]     // do usuniecia potem!
        public IActionResult Ip()
        {
            //string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();


            var data = System.Text.Encoding.UTF8.GetBytes("anrzej duda");
            //    var (publicKey, privateKey) = AsymmetricEncryptionHelper.GenerateKeys("przemek");
            //  var pubString = Convert.ToBase64String(publicKey.);
            //  var privString = Convert.ToBase64String(privateKey);
            var user = _unitOfWork.Users.Find<User>(203);
            var encryptedData = AsymmetricEncryptionHelper.Encrypt(data, user.PublicKey);
            var encString = Convert.ToBase64String(encryptedData);
            var decryptedData = AsymmetricEncryptionHelper.Decrypt(encryptedData, user.PrivateKey, "Farmer1998@");
            var decString = System.Text.Encoding.UTF8.GetString(decryptedData);









            return Ok(ipAddress);
        }
        /* [HttpGet("{errorType}")]
         public IActionResult Error(string errorType)
         {

             return View("Views/Shared/"+errorType+".cshtml");

                }

     */

        
        
    }
}