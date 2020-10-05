﻿using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PasswordManagerApp.Controllers;
using System.Security.Cryptography;
using System.Text;
using OtpNet;
using EmailService;
using Microsoft.AspNetCore.DataProtection;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;

namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {



        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataProtectionHelper dataProtectionHelper;
        private readonly IEmailSender _emailSender;


        public event EventHandler<Message> EmailSendEvent;


        public UserService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
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



        



        public bool VerifyEmail(string email)
        {

            return _unitOfWork.Users.CheckIfUserExist(email);
        }








        




        public User GetById(int id)
        {

            return _unitOfWork.Users.Find<User>(id);

        }

        


        
        

        #region private methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        
        
        private bool CheckUserGuidDeviceInDb(string GuidDeviceFromCookie, User authUser)
        {


            var GuidDeviceHashFromCookie = dataToSHA256(GuidDeviceFromCookie);


            if (_unitOfWork.Context.UserDevices.Any(ud => ud.User == authUser && ud.DeviceGuid == GuidDeviceHashFromCookie))
                return true;
            else
                return false;

        }
        
        private string dataToSHA256(string data)
        {
            SHA256 mysha256 = SHA256.Create();
            return Convert.ToBase64String(mysha256.ComputeHash(Encoding.UTF8.GetBytes(data)));

        }


        #endregion

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        


        /* private void ManageAuthorizedDevices(User authUser)
        {
            
            var c = cookieHandler.GetClientInfo();
            string browser = c.UA.Family.ToString() + " " + c.UA.Major.ToString();


           
            bool IsNewUserDevice = false;
            string guidDevice = "";
            bool IsUserGuidDeviceMatch = true;


            var deviceCookieExist = cookieHandler.CheckIfCookieExist("DeviceInfo");
            if (deviceCookieExist)
            {
                var GuidDeviceFromCookie = cookieHandler.ReadAndDecryptCookie("DeviceInfo");
                IsUserGuidDeviceMatch = CheckUserGuidDeviceInDb(GuidDeviceFromCookie, authUser);
                if (IsUserGuidDeviceMatch)
                    return;
            
            }
            if(deviceCookieExist==false || IsUserGuidDeviceMatch==false)
            {
                

                guidDevice = Guid.NewGuid().ToString();
                cookieHandler.CreateCookie("DeviceInfo", guidDevice, null);
                var userGuidDeviceHash = dataToSHA256(guidDevice);
                var ipMatchWithPrevious = CheckPreviousUserIp(authUser);
                 IsNewUserDevice = AddNewDeviceToDb(userGuidDeviceHash, authUser);
                
                
                if( ipMatchWithPrevious )
                    IsNewUserDevice = false;
                
                
                   
                
               
                
                

            }
            



            

         
            if (IsNewUserDevice)
                EmailSendEvent?.Invoke(this, 
                new Message(new string[] { authUser.Email }, "Nowe urządzenie " + c.OS.ToString(), "Zarejestrowano logowanie z nowego adresu ip: "+GetUserIpAddress(authUser)+", system : " + c.OS.ToString() + " " + browser + " dnia " + DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd' 'HH:mm:ss") + ".")
                );
         




        } */

        public void InformAllUsersAboutOldPasswords()
        {

            var allloginDatasList = _unitOfWork.Context.LoginDatas.ToList();
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


                _emailSender.SendEmailAsync(new Message(new string[] { userEmail }, "PasswordManagerApp stare hasła", message));


            }








        }
        public void InformUserAboutOldPasswords(int userId)
        {
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
        }



        
        









        



       


       



       
    }

}