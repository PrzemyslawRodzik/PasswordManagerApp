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
        User Authenticate(string email, string password);
        

        ClaimsIdentity GetClaimIdentity(User authUser);
        User Create(string email, string password);
        void Update(User user, string password= null);
         bool ChangeMasterPassword(string password,string authUserId);
        IEnumerable<User> GetAll();

        
        bool VerifyEmail(string email);
        User GetById(int id);
        void Delete(int id);
        void SendTotpToken(User authUser);
        int VerifyTotpToken(User authUser, string token);

        Task<User> AuthenticateExternal(string id);
        Task<User> AddExternal(string id, string email);
        void UpdatePreferences(UpdatePreferencesWrapper upPreferences,int userId);



          bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}