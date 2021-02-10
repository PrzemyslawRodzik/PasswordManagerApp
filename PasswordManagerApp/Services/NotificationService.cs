using Microsoft.AspNetCore.SignalR;
using PasswordManagerApp.Controllers;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
using PasswordManagerApp.RealTimeAlerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordManagerApp.Services
{
    public class NotificationService
    {

        private readonly EncryptionService _encryptService;
        private readonly IHubContext<NotificationHub> _signalRContext;
        private readonly ApiService _apiService;
        
        

        

        public NotificationService(EncryptionService encryptService, IHubContext<NotificationHub> signalRContext, ApiService apiService)
        {

            _encryptService = encryptService;
            _signalRContext = signalRContext;
            _apiService = apiService;

        }


        #region Events
        public async void DataViewEvent(IPasswordModel e){ 
            int isPwned = PwnedPasswords.IsPasswordPwnedAsync(e.Password, new CancellationToken(), null).Result; 
            if(isPwned>0)
                await SendMessageToClient(e.UserId.ToString(), $"The password you're using on {e.Name} site has previously appeared in a data breach. You should consider changing it.");
        }
        public async void DataEditEvent(ICompromisedModel e)
        {
            if (e.Compromised == 1)
                await SendMessageToClient(e.UserId.ToString(), $"The password you're added on {e.Name} site has previously appeared in a data breach. You should consider changing it.");
        }
        public async void UserPasswordSaveEvent(UserCredentials e)
        {
            InformAboutMasterPasswordBreach(e.UserId, e.Password);
            await Task.Delay(7000);
            InformUserAboutPasswordsBreach(e.UserId);
        }
        #endregion


        #region Checking methods

        public async void InformUsersAboutOldPasswords()
        {
            var activeUsers = _encryptService.GetActiveUsers();
            var userMessages = _apiService.CheckOutOfDate(activeUsers);

            foreach (KeyValuePair<string, string> x in userMessages)
            {
                await SendMessageToClient(x.Key, x.Value);
            }
        }


        public async void InformUsersAboutPasswordsBreach() {
            var activeUsers = _encryptService.GetActiveUsers();
            List<LoginData> logins, loginsToUpdate = new List<LoginData>();
            string password, loginNames, message;
            foreach (string userId in activeUsers)
            {
                loginNames = "";
                logins = _apiService.GetAllUserData<LoginData>(Int32.Parse(userId), compromised: 0).Result?.ToList();
                if (logins is null || logins.Count <= 0)
                    continue;
                foreach(var login in logins)
                {
                    password = login.Password;
                    password = _encryptService.Decrypt(userId, password);
                    if (PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result > 0)
                    {
                        loginNames += login.Name + ", ";
                        login.Compromised = 1;
                        loginsToUpdate.Add(login);
                        
                    }      
                }
                if (!loginNames.Equals(""))
                {
                    if(loginsToUpdate.Count>0)
                        await _apiService.UpdateMany<LoginData>(loginsToUpdate);
                    message = $"We are detected new breached passwords for {loginNames} accounts. You should consider changing them.";
                    await SendMessageToClient(userId, message);
                }
                loginsToUpdate.Clear();}



        }
        public async void InformUserAboutPasswordsBreach(string userId)
        {
            
            List<LoginData> logins, loginsToUpdate = new List<LoginData>();
            string password, loginNames, message;
            
                loginNames = "";
                logins = _apiService.GetAllUserData<LoginData>(Int32.Parse(userId), compromised: 0).Result?.ToList();
                if (logins is null || logins.Count <= 0)
                    return;
                foreach (var login in logins)
                {
                    password = login.Password;
                    password = _encryptService.Decrypt(userId, password);
                    if (PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result > 0)
                    {
                        loginNames += login.Name + ", ";
                        login.Compromised = 1;
                        loginsToUpdate.Add(login);

                    }
                }
                if (!loginNames.Equals(""))
                {
                    if (loginsToUpdate.Count > 0)
                        await _apiService.UpdateMany<LoginData>(loginsToUpdate);
                    message = $"We are detected new breached passwords for {loginNames} accounts. You should consider changing them.";
                    await SendMessageToClient(userId, message);
                }
                loginsToUpdate.Clear();
            



        }
        public async void InformAboutMasterPasswordBreach(string userId,string password)
        {
            var isBreached = await PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null);
            if (isBreached <= 0)
                return;
            await _apiService.UpdatePasswordStatus(Int32.Parse(userId), compromised:1);
            await SendMessageToClient(userId, "Your master password was compromised " + isBreached + " times. Please change your main password.");
        }
       
        public async Task SendMessageToClient(string userId,string message)
        {
            await _signalRContext.Clients.User(userId).SendAsync("ReceiveAlert", message);
        }

        #endregion



        private void ApiService_DataEditEvent(object sender, ICompromisedModel e)
        {
            var message = $"The password you're added on {e.Name} site has previously appeared in a data breach. You should consider changing it.";


            if (e.Compromised == 1)
                // SendMessageToClient(e.UserId.ToString(), message).RunSynchronously();


                _signalRContext.Clients.User(e.UserId.ToString()).SendAsync("ReceiveAlert", message);
        }


    }
}
