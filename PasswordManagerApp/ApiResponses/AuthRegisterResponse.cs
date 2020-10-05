﻿using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.ApiResponses
{
    public class AuthRegisterResponse
    {
        public AccessToken AccessToken { get; set; }

        public User User { get; set; }

    }
}
