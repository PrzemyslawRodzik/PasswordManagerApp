﻿using EmailService;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public interface IUserService
    {
        event EventHandler<Message> EmailSendEvent;
        
        int GetAuthUserId();


        
        


        bool VerifyEmail(string email);
        User GetById(int id);
        
        



        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);

        void InformUserAboutOldPasswords(int userId);
        void InformAllUsersAboutOldPasswords();
        
        
    }
}