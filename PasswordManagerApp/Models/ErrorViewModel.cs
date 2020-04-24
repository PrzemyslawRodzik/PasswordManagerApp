
using Microsoft.AspNetCore.Mvc;
using System;

namespace PasswordManagerApp.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

     
    }
}
