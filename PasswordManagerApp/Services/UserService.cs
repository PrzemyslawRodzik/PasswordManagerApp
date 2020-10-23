﻿using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


using Microsoft.AspNetCore.DataProtection;
using PasswordManagerApp.Handlers;


namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataProtectionHelper dataProtectionHelper;
        

        public UserService(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
        {
            
            _httpContextAccessor = httpContextAccessor;
            dataProtectionHelper = new DataProtectionHelper(provider);

        }



        
        public int GetAuthUserId()
        {

            try
            {
                int id = Int32.Parse(_httpContextAccessor.HttpContext.User.Identity.Name);

                return id;
            }
            catch (NullReferenceException)
            {
                return -1;
            }

        }
        
        

        public void InformAllUsersAboutOldPasswords()
        {

           /* var allloginDatasList = _unitOfWork.Context.LoginDatas.ToList();
            if (allloginDatasList is null)
                return;
            var loginDatasListWithOldPasswords = allloginDatasList.Where(x => (DateTime.UtcNow.ToLocalTime() - x.ModifiedDate).Days >= 30).ToList();
            if (loginDatasListWithOldPasswords is null)
                return;
            string websitesList = "";
            foreach (var item in loginDatasListWithOldPasswords.GroupBy(x => x.UserId))
            {
                websitesList = "";
                var userEmail = GetById(item.Key).Email;
                item.ToList().ForEach(x => websitesList += x.Website + ", ");


                string message = $"wykryto {item.Count()} hasła nie zmieniane od 30 dni dla podanych stron internetowych : {websitesList}!";


              //  _emailSender.SendEmailAsync(new Message(new string[] { userEmail }, "PasswordManagerApp stare hasła", message));


            }



        */




        }
        public void InformUserAboutOldPasswords(int userId)
        {
            
           /*
            string userEmail = GetById(userId).Email;
            var allUserLoginData = _unitOfWork.Context.LoginDatas.Where(x => x.UserId == userId).ToList();
            if (allUserLoginData is null)
                return;
            var loginDataListWithOldPasswords = allUserLoginData.Where(x => (DateTime.UtcNow.ToLocalTime() - x.ModifiedDate).Days >= 30).ToList();
            if (loginDataListWithOldPasswords is null)
                return;
            string websitesList = "";

            loginDataListWithOldPasswords.ForEach(x => websitesList += x.Website + ", ");

            string message = $"wykryto {loginDataListWithOldPasswords.Count} hasła nie zmieniane od 30 dni dla podanych stron internetowych : {websitesList}.";

            //  _emailSender.SendEmailAsync(new Message(new string[] { userEmail },"PasswordManagerApp stare hasła", message));
            */
        }
 
    }

}