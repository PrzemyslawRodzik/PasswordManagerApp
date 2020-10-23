using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public interface IUserService
    {
       // event EventHandler<Message> EmailSendEvent;
        
        int GetAuthUserId();

        void InformUserAboutOldPasswords(int userId);
        void InformAllUsersAboutOldPasswords();
        
        
    }
}