using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public interface IUserService
    {
        event EventHandler<string> EmailSendEvent;
        User Authenticate(string email, string password);

        ClaimsIdentity GetClaimIdentity(User authUser);
        User Create(string email, string password);
        void Update(User user, string password= null);
        IEnumerable<User> GetAll();

        
        
        User GetById(int id);
        void Delete(int id);

        Task<User> AuthenticateExternal(string id);
        Task<User> AddExternal(string id, string email);
        




    }
}
