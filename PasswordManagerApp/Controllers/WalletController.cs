using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using System.Linq;

namespace PasswordManagerApp.Controllers
{  
   // [Authorize]    pamiętać by odkomentować !
    [Route("user")]
    public class WalletController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
            public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        [Route("dashboard")]
        [Route("~/")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(controllerName: "Auth", actionName: "Login");


            var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));

            var countLogin = _unitOfWork.Wallet.GetDataCountForUser<LoginData>(user);
            var countCreditCards = _unitOfWork.Wallet.GetDataCountForUser<CreditCard>(user);

            ViewBag.countCreditCards = countCreditCards;

          var countPaypall =  _unitOfWork.Wallet.GetDataCountForUser<PaypallAcount>(user);

            ViewBag.countPasswords = countLogin + countPaypall;

           var countPaypalComp =  _unitOfWork.Wallet.GetDataBreachForUser<PaypallAcount>(user);

           var countLoginComp =  _unitOfWork.Wallet.GetDataBreachForUser<LoginData>(user);
            ViewBag.countCompromised = countPaypalComp + countLoginComp;

            ViewBag.countSharedData = _unitOfWork.Wallet.GetDataCountForUser<SharedLoginData>(user);

            return View("Views/Wallet/IndexDashboard.cshtml");

        }


        [Route("list")]
        public IActionResult List()
        {

            var user = _unitOfWork.Users.Find<User>(int.Parse(User.Identity.Name));
            var loginData = _unitOfWork.Wallet.FindByCondition<LoginData>(x => x.User == user);

            return View("Views/Wallet/ListItem.cshtml", loginData);
        }
        [HttpGet("{errorType}")]
        public IActionResult Error(string errorType)
        {

            return View("Views/Shared/"+errorType+".cshtml");

        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyLogin(string website,string login,int id)
        {
            if (_unitOfWork.Context.LoginDatas.Any(l => l.Website == website && l.Login == login))
            {
                return Json($"You already set login {login} for that website {website}");
            }

            return Json(true);
        }



        [Route("create")]
        [HttpGet]
        public IActionResult LoginDataCreate()
        {
            return View(new LoginDataViewModel());
        }
        [Route("create")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LoginDataCreate(LoginDataViewModel model)
        {
            if (!ModelState.IsValid)

                return View(model);



            return Ok("true");



        }
        [Route("/ip")]
        public IActionResult ip()
        {
            //string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
           
            
            return Ok(ipAddress);
        }



    }
}