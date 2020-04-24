using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    interface IUserService
    {
        Task<User> Authenticate(string email, string password);
        Task<User> Add(string email, string password);

        Task<User> AuthenticateExternal(string id);
        Task<User> AddExternal(string id,string email);


    }
}
