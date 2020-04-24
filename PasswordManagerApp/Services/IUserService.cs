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
        User Create(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void delete(int id);

        Task<User> AuthenticateExternal(string id);
        Task<User> AddExternal(string id, string email);
        void  CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
       


    }
}
