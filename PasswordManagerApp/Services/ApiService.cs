using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PasswordManagerApp.ApiResponses;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UAParser;

namespace PasswordManagerApp.Services
{
    public class ApiService
    {
        public event EventHandler<ICompromisedModel> DataEditEvent;
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public ApiService(HttpClient client, IHttpContextAccessor httpContextAccessor)
         {
            _httpContextAccessor = httpContextAccessor;
            client.DefaultRequestHeaders.Authorization = GetAuthJwtTokenFromCookie();
            _client = client;
            
        }

        private AuthenticationHeaderValue GetAuthJwtTokenFromCookie()
        {
            try
            {
                
               return  new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.User.FindFirst("Access_token")?.Value);
            }
            catch (Exception)
            {
                return new AuthenticationHeaderValue("Bearer", "");
            }
        }
            
            

        public  bool CheckEmailAvailability(string email)
        {
            
            var response = _client.GetAsync($"users/email/check?email={email}").Result;

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<Dictionary<string,int>> GetUserStatisticData()
        {
            
            var response = await _client.GetAsync("statistics/user-data");
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Dictionary<string,int>>(responseString);
        }

        public Dictionary<string, string> CheckOutOfDate(string[] activeUsers)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(activeUsers), Encoding.UTF8, "application/json");
            response = _client.PostAsync("logindatas/check-out-of-date", content).Result;

            var responseString =  response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
        }

        public async Task<bool> UpdateMany<T>(List<T> models)
        {
            HttpResponseMessage response;
            string typeName = typeof(T).Name;
            typeName += "s";
            StringContent content = new StringContent(JsonConvert.SerializeObject(models), Encoding.UTF8, "application/json");
            
             response = await _client.PutAsync($"{typeName}/updateMany", content);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
            

        }

        public Dictionary<string, string> CheckPasswordBreach(string[] activeUsers)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(activeUsers), Encoding.UTF8, "application/json");
            response = _client.PostAsync("logindatas/check-password-breach", content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
        }



        #region Generic CRUD api calls

        public async Task<IEnumerable<T>> GetAllUserData<T>(int userId, int? compromised = null,int? expired=null) where T : class
        {
            string typeName = typeof(T).Name;
            typeName += "s";
            var response = await _client.GetAsync($"{typeName}?userId={userId}&compromised={compromised}&expired={expired}");
            
            response.EnsureSuccessStatusCode();
            

            var responseString = await response.Content.ReadAsStringAsync();
                
            return  JsonConvert.DeserializeObject<IEnumerable<T>>(responseString);

        }
        public IEnumerable<SharedLoginModel> GetSharedImportLogins(int userId)
        {

            var response = _client.GetAsync($"logindatas/share?userId={userId}").Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<SharedLoginModel>>(responseString);
        }
        public IEnumerable<ShareLoginModel> GetSharedExportLogins(int userId)
        {

            var response = _client.GetAsync($"logindatas/share/export?userId={userId}").Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<ShareLoginModel>>(responseString);
        }




        public  bool CheckUserGuidDeviceInDb(string guidDeviceHash,int userId)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(new {GuidDevice = guidDeviceHash, UserId = userId }), Encoding.UTF8, "application/json");
            response = _client.PostAsync("users/devices/check-guid", content).Result;
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public  Task UpdatePasswordStatus(int userId, int compromised)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(new { UserId = userId, Compromised = compromised }), Encoding.UTF8, "application/json");
            response = _client.PostAsync("users/update-password-status", content).Result;
            return Task.CompletedTask;
            
        }

        
        public bool  CheckLoginDuplicate(string website, string login)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(new { Website = website, Login = login}), Encoding.UTF8, "application/json");
            response = _client.PostAsync("logindatas/checklogindup", content).Result;
            if (response.IsSuccessStatusCode)
                return false;
            return true;
          
        }

        public void HandleNewDeviceLogIn(string ipAddress, string guidDevice,int userId, string osName, string browserName)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(new { IpAddress = ipAddress, GuidDevice = guidDevice, UserId = userId, OSName = osName, BrowserName = browserName }), Encoding.UTF8, "application/json");
            response = _client.PostAsync("users/devices/authorize-new-device", content).Result;
           
        }

        public async Task<T> GetDataById<T>(int id) where T : class
        {
            string typeName = typeof(T).Name;
            typeName += "s";
            var response = await _client.GetAsync($"{typeName}/{id}");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseString);
        }

       

        public async Task<T> CreateUpdateData<T>(T entity, int? id=null) where T : class
        {
            HttpResponseMessage response;
            string typeName = typeof(T).Name;
            typeName += "s";
            StringContent content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            if(id==null)
                response = await _client.PostAsync($"{typeName}/", content);
            else
                response = await _client.PutAsync($"{typeName}/{id}", content);
            
            response.EnsureSuccessStatusCode();
            if(entity is ICompromisedModel)
                OnDataEditEvent(entity as ICompromisedModel);

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        

        public async Task<bool> DeleteData<T>(int id)
        {
            HttpResponseMessage response;
            string typeName = typeof(T).Name;
            typeName += "s";
            
            response = await _client.DeleteAsync($"{typeName}/{id}");

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;

            

            
        }
        public async Task<bool> DeleteSharedData(int id)
        {
            HttpResponseMessage response;
            

            response = await _client.DeleteAsync($"logindatas/share/{id}");

            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;




        }

        public  async Task<ApiResponse> ChangeMasterPassword(PasswordChangeViewModel model)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("users/password/change", content);
            

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse>(responseString);
        }

        public async Task<AuthResponse> UpdateUserPreferences(string switch2F, string switchPnot, string sliderVerTime)
        {
            HttpResponseMessage response;
           
            StringContent content = new StringContent(JsonConvert.SerializeObject(new { TwoFactor = switch2F, PassNotifications = switchPnot, VerificationTime = sliderVerTime }), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("users/update/preferences", content);
            

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthResponse>(responseString);
        }

        #endregion

        #region Authentication api calls

        

        public async Task<AuthResponse> LogIn(LoginViewModel model)
        {
            HttpResponseMessage response;
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            
            response = await _client.PostAsync($"identity/login", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthResponse>(responseString);


        }

        public async Task<AuthResponse> RegisterUser(RegisterViewModel model)
        {
            HttpResponseMessage response;

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            response = await _client.PostAsync($"identity/register", content);
            var responseString = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<AuthResponse>(responseString);
        }

        public User GetAuthUser()
        {

            var response = _client.GetAsync("users/getauthuser").Result;
            
            if (response.StatusCode.Equals(StatusCodes.Status404NotFound))
                return null;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<User>(responseString);
        }
        public async Task<ApiResponse> DeleteAccountProcess(DeleteAccountViewModel model,string step)
        {
            HttpResponseMessage response;

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            response = await _client.PostAsync($"users/deleteaccount{step}",content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse>(responseString);
   
        }

        public async Task<ApiTwoFactorResponse> TwoFactorLogIn(int userId, string token)
        {
            
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(new {userId = userId, token = token }) , Encoding.UTF8, "application/json");
            response = await _client.PostAsync($"identity/twofactorlogin", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiTwoFactorResponse>(responseString);
        }

        public async Task<ApiResponse>  SendTotpByEmail(int idUser)
        {
            HttpResponseMessage response;
            StringContent content = new StringContent(JsonConvert.SerializeObject(idUser), Encoding.UTF8, "application/json");
            response = await _client.PostAsync($"identity/twofactorlogin/resendtotp", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse>(responseString);
        }

        #endregion

        

        public  void OnDataEditEvent(ICompromisedModel model)
        {
            
           // DataEditEvent?.Invoke(this, model);
        }
    }
}
