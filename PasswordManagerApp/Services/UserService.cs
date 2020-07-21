using Microsoft.AspNetCore.Http;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PasswordManagerApp.Controllers;
using UAParser;
using System.Security.Cryptography;
using System.Text;
using OtpNet;
using EmailService;
using Microsoft.AspNetCore.DataProtection;
using PasswordManagerApp.Handlers;

namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {

        public event EventHandler<Message> EmailSendEvent;

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtectionProvider _provider;
        public CookieHandler cookieHandler;


        public UserService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider provider)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _provider = provider;
            cookieHandler = new CookieHandler(new HttpContextAccessor(), _provider);

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

            EmailSendEvent?.Invoke(this, new Message(new string[] { user.Email }, "Zalozyles konto na PasswordManager.com", "Witamy w PasswordManager web api " + user.Email));

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
            if (!VerifyPasswordHash(password, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.PasswordSalt)))
                return null;

            // authentication successful



            return user;
        }



        public ClaimsIdentity GetClaimIdentity(User authUser)
        {



            ManageAuthorizedDevices(authUser);









            var claims = new List<Claim>
{           new Claim(ClaimTypes.Name,authUser.Id.ToString()),
            new Claim(ClaimTypes.Email,authUser.Email),
            new Claim("Admin", authUser.Admin.ToString()),
            new Claim("TwoFactorAuth", authUser.TwoFactorAuthorization.ToString())



        };
            return new ClaimsIdentity(claims, "CookieAuth");

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

        #region private methods
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






        private string GenerateTotpToken(User authUser)
        { string totpToken;
            string sysKey = "ajskSJ62j%sjs.;'[ah1";
            var key_b = Encoding.UTF8.GetBytes(sysKey + authUser.Email);


            Totp totp = new Totp(secretKey: key_b, mode: OtpHashMode.Sha512, step: 300, timeCorrection: new TimeCorrection(DateTime.UtcNow));
            totpToken = totp.ComputeTotp(DateTime.UtcNow);


            return totpToken;
        }
        private void SaveToDb(User authUser, string totpToken)
        {
            _db.Totp_Users.Add(
                new Totp_user() {
                    Token = totpToken,
                    Create_date = DateTime.UtcNow.ToLocalTime(),
                    Expire_date = DateTime.UtcNow.AddSeconds(300).ToLocalTime(),
                    User = authUser




                });
            _db.SaveChanges();
        }
        #endregion

        public bool AddNewDeviceToDb(string newOsHash, User authUser)
        {



            if (!_db.UserDevices.Any(b => b.User == authUser && b.DeviceGuid == newOsHash))

            {

                UserDevice usd = new UserDevice();
                usd.User = authUser;
                usd.Authorized = 1;
                usd.DeviceGuid = newOsHash;

                _db.UserDevices.Add(usd);
                _db.SaveChanges();


                return true;


            }
            return false;
        }
       

        private void ManageAuthorizedDevices(User authUser)
        {
            string uaString = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);

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
                
                IsNewUserDevice = AddNewDeviceToDb(userGuidDeviceHash, authUser);

            }






            if (IsNewUserDevice)
                EmailSendEvent?.Invoke(this, new Message(new string[] { authUser.Email }, "Nowe urządzenie " + c.OS.ToString(), "Zarejestrowano logowanie z nowego systemu : " + c.OS.ToString() + " " + browser + " dnia " + DateTime.UtcNow.ToLocalTime().ToString() + ". Urządzenie z podanym systemem OS zostało dodane do listy zaufanych urządzeń"));




        }
        private bool CheckUserGuidDeviceInDb(string GuidDeviceFromCookie, User authUser)
        {

            
            var GuidDeviceHashFromCookie = dataToSHA256(GuidDeviceFromCookie);

            if (_db.UserDevices.Any(ud => ud.User == authUser && ud.DeviceGuid == GuidDeviceHashFromCookie))
                return true;
            else
                return false;

        }










        public void SendTotpToken(User authUser)
        {
            string totpToken = GenerateTotpToken(authUser);
            SaveToDb(authUser, totpToken);
            EmailSendEvent?.Invoke(this, new Message(new string[] { authUser.Email }, "Jednorazowy kod dostępu", "Jednorazowy kod dostępu do konta: " + totpToken + " dla uzytkownika: " + authUser.Email+ " Podany kod musisz wprowadzic w ciagu 5min"));


        }
        private enum ResultsToken
        {
            NotMatched,
            Matched,
            Expired,
        }


        public int VerifyTotpToken(User authUser,string totpToken)
        {
            
            var totpTokenHash = Totp_user.TotpHashBase64(totpToken);
            string sysKey = "ajskSJ62j%sjs.;'[ah1";
            
            long lastUse;
            Totp totp = new Totp(secretKey: Encoding.UTF8.GetBytes(sysKey + authUser.Email), mode: OtpHashMode.Sha512, step: 300,timeCorrection:new TimeCorrection(DateTime.UtcNow));
            
            
            var activeTokenRecordFromDb = _db.Totp_Users.Where(b => b.User == authUser && b.Token == totpTokenHash).FirstOrDefault();
            if (activeTokenRecordFromDb != null)
            {
               // activeTokenRecordFromDb.Active = 0;
               
                if (activeTokenRecordFromDb.Expire_date >= DateTime.UtcNow.ToLocalTime())
                {
                    return totp.VerifyTotp(totpToken, out lastUse,window:new VerificationWindow(1,1)) ? (int)ResultsToken.Matched:(int)ResultsToken.NotMatched; 
                }
                else
                {
                    return (int)ResultsToken.Expired;
                }

            }
            { 
                return (int)ResultsToken.NotMatched;
            }

        }

        private string dataToSHA256(string data)
        {
            SHA256 mysha256 = SHA256.Create();
            return Convert.ToBase64String(mysha256.ComputeHash(Encoding.UTF8.GetBytes(data)));
            
        }
























    }
}
