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

        public IEnumerable<User> Get2MethodFromIUserRepository()
        {
            throw new NotImplementedException();
        }
    }
}