﻿using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


using Microsoft.AspNetCore.DataProtection;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.RealTimeAlerts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {

        public UserService()
        {

            
            
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
        
 
    }

}