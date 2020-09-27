﻿using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public interface IUserRepository :IRepositoryBase
    {
        bool CheckIfUserExist(string email);
        public string GetActiveToken(User authUser);
        public bool IsTokenActive(User authUser);
    }
}