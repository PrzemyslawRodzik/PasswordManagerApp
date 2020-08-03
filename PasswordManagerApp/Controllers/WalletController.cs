using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;


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



    }
}