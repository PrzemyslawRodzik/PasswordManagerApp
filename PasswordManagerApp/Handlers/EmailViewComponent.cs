using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PasswordManagerApp.Handlers
{
    public class EmailViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value;
            ViewBag.Email = email;
            return View("~/Views/Shared/EmailPartialView.cshtml");
        }
    }
}
