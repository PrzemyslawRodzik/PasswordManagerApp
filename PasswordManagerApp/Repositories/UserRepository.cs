﻿using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }


        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }





        public bool CheckIfUserExist(string email)
        {
            return ApplicationDbContext.Users.Any(u => u.Email == email);
         
        }
        public string GetActiveToken(User authUser)
        {
            var currentTime = DateTime.UtcNow;
            var model = ApplicationDbContext.Totp_Users.Where(x => x.UserId == authUser.Id && x.Expire_date > currentTime).FirstOrDefault();
            return model.Token;
        }


        public bool IsTokenActive(User authUser)
        {
            var currentTime = DateTime.UtcNow;
            bool isActive = ApplicationDbContext.Totp_Users.Any(x => x.UserId == authUser.Id && x.Expire_date > currentTime);

            if (isActive)
                return true;
            else
                return false;
        }

    }
}