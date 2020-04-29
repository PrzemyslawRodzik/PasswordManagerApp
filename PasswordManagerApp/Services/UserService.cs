using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PasswordManagerApp.Controllers;
using UAParser;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {

        public event EventHandler<string> EmailSendEvent;

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public UserService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;

        }

       
            public User Create(string email, string password)
            {
                // validation
                if (string.IsNullOrWhiteSpace(password))
                    throw new AppException("Password is required");

                if (_db.Users.Any(x => x.Email == email))
                    throw new AppException("Email \"" + email + "\" is already taken");

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

            User user = new User();
                user.Email = email;
            user.Password = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = Convert.ToBase64String(passwordSalt);



            _db.Users.Add(user);
                _db.SaveChanges();

            EmailSendEvent?.Invoke(this, "Wysylam e-maila ...");

            return user;
            }

        public void Update(User user, string password = null)
        {
            throw new NotImplementedException();
        }








        public User Authenticate(string email, string password)
            {
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                    return null;

                var user = _db.Users.SingleOrDefault(x => x.Email == email);

                // check if username exists
                if (user == null)
                    return null;

                // check if password is correct
                if (!VerifyPasswordHash(password,Convert.FromBase64String(user.Password) , Convert.FromBase64String(user.PasswordSalt)))
                    return null;

            // authentication successful

            

            return user;
            }

        public ClaimsIdentity GetClaimIdentity(User authUser)
        {
            string uaString = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            string cookieOsHash;

            using (SHA256 mysha256 = SHA256.Create())
            {
                
                 
                cookieOsHash = Convert.ToBase64String(mysha256.ComputeHash(Encoding.UTF8.GetBytes(c.OS.ToString())));
            }


            var newUserDevice = AddNewDevice(cookieOsHash, authUser);
            if (newUserDevice)
                EmailSendEvent?.Invoke(this, "text");



            
            




            var claims = new List<Claim>
{           new Claim(ClaimTypes.Name,authUser.Id.ToString()),
            new Claim(ClaimTypes.Email,authUser.Email),
            new Claim("Admin", "false"),
            new Claim("TwoFactorAuth", "false"),
            new Claim("OsHash", cookieOsHash )
            

        };
            return new ClaimsIdentity(claims, "UserCookie");

        }

        

       

      

        public IEnumerable<User> GetAll()
        {
            return _db.Users;
        }


        

        public User GetById(int id)
        {
            return _db.Users.Find(id);
        }

        public void Delete(int id)
        {
           

                var user = _db.Users.Find(id);
           

            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }
       
        
        public Task<User> AuthenticateExternal(string id)
        {
            throw new NotImplementedException();
        }

        public Task<User> AddExternal(string id, string email)
        {
            throw new NotImplementedException();
        }

        #region private  hashing helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
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

        public  bool AddNewDevice(string newOsHash,User authUser)
        {
            


              if (!_db.UserDevices.Any(b => b.User == authUser && b.CookieDeviceHash == newOsHash))

            { 
            
                UserDevice usd = new UserDevice();
                usd.User = authUser;
                usd.Authorized = 1;
                usd.CookieDeviceHash = newOsHash;

                _db.UserDevices.Add(usd);
                _db.SaveChanges();

                return true;


            }
            return false;









        }













        #endregion


    }
}
