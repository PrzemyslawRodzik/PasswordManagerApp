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

namespace PasswordManagerApp.Services
{
    public class ApiService
    {

        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient client, IHttpContextAccessor httpContextAccessor)
         {
            _httpContextAccessor = httpContextAccessor;
            client.DefaultRequestHeaders.Authorization = GetAuthJwtTokenFromCookie();
            _client = client;
        }

        private AuthenticationHeaderValue GetAuthJwtTokenFromCookie() 
            => 
            new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.User.FindFirst("Access_token")?.Value);

        public  bool CheckEmailAvailability(string email)
        {
            
            var response = _client.GetAsync($"users/check?email={email}").Result;

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        #region Generic CRUD api calls

        public async Task<IEnumerable<T>> GetAllUserData<T>(int userId) where T : class
        {
            string typeName = typeof(T).Name;
            typeName += "s";
            var response = await _client.GetAsync($"{typeName}?userId={userId}");
            
            response.EnsureSuccessStatusCode();

             var responseString = await response.Content.ReadAsStringAsync();
                
            return  JsonConvert.DeserializeObject<IEnumerable<T>>(responseString);

        }
        public async Task<T> GetDataById<T>(int userId) where T : class
        {
            string typeName = typeof(T).Name;
            typeName += "s";
            var response = await _client.GetAsync($"{typeName}/{userId}");

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
            response = await _client.PostAsync("users/update-preferences", content);
            

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthResponse>(responseString);
        }

        #endregion

        #region Authentication api calls

        public async Task<AuthResponse> GetAccessToken(LoginViewModel model)
        {
            HttpResponseMessage response;

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            response = await _client.PostAsync("identity/token", content);


            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthResponse>(responseString);
        }

        public async Task<User> AuthenticateUser(LoginViewModel model)
        {
            HttpResponseMessage response;
            
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            
            response = await _client.PostAsync($"identity/authenticate", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode==HttpStatusCode.BadRequest)
                return null;
            else
                return JsonConvert.DeserializeObject<User>(responseString);


        }

        public async Task<AuthRegisterResponse> RegisterUser(RegisterViewModel model)
        {
            HttpResponseMessage response;

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            response = await _client.PostAsync($"identity/register", content);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthRegisterResponse>(responseString);
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
            response = await _client.PostAsync($"identity/resendtotp", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse>(responseString);
        }

        #endregion
    }
}
