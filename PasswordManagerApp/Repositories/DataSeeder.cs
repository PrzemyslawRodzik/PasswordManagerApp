using Bogus;
using PasswordManagerApp.Models;
using System;
using System.Linq;


namespace PasswordManagerApp.Repositories
{
    public static class DataSeeder
    {

        

        public static void SeedData(ApplicationDbContext applicationDbContext)
        {

            
            UnitOfWork unitOfWork = new UnitOfWork(applicationDbContext);
            
            
            if (unitOfWork.Context.Users.Any() || unitOfWork.Context.LoginDatas.Any() || unitOfWork.Context.CreditCards.Any() || unitOfWork.Context.PersonalInfos.Any())
                return;
            
            byte[] passwordSalt;
            
            var testUsers = new Faker<User>()
                .RuleFor(u => u.AuthenticationTime, 1)
                .RuleFor(u => u.TwoFactorAuthorization, 0)
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
                .RuleFor(u => u.Password, CreatePasswordHash("zxczxc", out passwordSalt))
                .RuleFor(u => u.PasswordSalt, Convert.ToBase64String(passwordSalt))
                .RuleFor(u => u.PasswordNotifications, 1)
                .RuleFor(u => u.Admin, 0);




            var users = testUsers.Generate(200);


            var testLoginData = new Faker<LoginData>()
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
                .RuleFor(u => u.Password, f=>f.Internet.Password())
                .RuleFor(u => u.Login, f => f.Internet.UserName())
                .RuleFor(u => u.Name, f => f.Internet.DomainName())
                .RuleFor(u => u.Compromised, f => f.Random.Number(0, 1))
                .RuleFor(u => u.Website, f => f.Internet.Url())
                .RuleFor(u => u.ModifiedDate, f => f.Date.Between(DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime().AddDays(26)))
                .RuleFor(u => u.User, f => f.PickRandom<User>(users));


            var loginDatas = testLoginData.Generate(800);

            var testPaypallAccounts = new Faker<PaypallAcount>()
               .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
               .RuleFor(u => u.Password, f => f.Internet.Password())
               
               
               .RuleFor(u => u.Compromised, f => f.Random.Number(0, 1))
               
               .RuleFor(u => u.ModifiedDate, f => f.Date.Between(DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime().AddDays(26)))
               .RuleFor(u => u.User, f => f.PickRandom<User>(users));


            var paypallAccounts = testPaypallAccounts.Generate(100);




            var testPersonalInfos = new Faker<PersonalInfo>()
               .RuleFor(u => u.Name, f => f.Name.FirstName())
               .RuleFor(u => u.SecondName, f => f.Name.FirstName())


               .RuleFor(u => u.LastName, f => f.Name.LastName())

               .RuleFor(u => u.DateOfBirth, f => f.Date.Between(new DateTime(1990, 6, 1, 7, 47, 0).ToLocalTime().Date, DateTime.Now.ToLocalTime().Date.AddYears(10)))
               .RuleFor(u => u.User, f => f.PickRandom<User>(users));
               


            var personalInfos = testPersonalInfos.Generate(100);
           
            var testAddressData = new Faker<Models.Address>()
              .RuleFor(u => u.AddressName, f => f.Address.County())
              .RuleFor(u => u.Street, f => f.Address.StreetName())


              .RuleFor(u => u.ZipCode, f => f.Address.ZipCode())
              .RuleFor(u => u.City, f => f.Address.City())
              .RuleFor(u => u.Country, f => f.Address.Country())


              .RuleFor(u => u.PersonalInfo, f => f.PickRandom<PersonalInfo>(personalInfos));


            var addressData = testAddressData.Generate(120);

            var testCreditCardsData = new Faker<Models.CreditCard>()
              .RuleFor(u => u.Name, f => f.Finance.AccountName())
              .RuleFor(u => u.CardHolderName, f => f.Name.FirstName())


              .RuleFor(u => u.CardNumber, f => f.Finance.CreditCardNumber())
              .RuleFor(u => u.SecurityCode, f => f.Finance.CreditCardCvv())
              .RuleFor(u => u.ExpirationDate, f => f.Date.Between(new DateTime(2018, 6, 1, 7, 47, 0).ToLocalTime().Date, DateTime.Now.ToLocalTime().Date.AddYears(2)).Date.ToString())



              .RuleFor(u => u.User, f => f.PickRandom<User>(users));


            var creditCardsData = testCreditCardsData.Generate(120);







            unitOfWork.Users.AddRange<User>(users);
            unitOfWork.Wallet.AddRange<LoginData>(loginDatas);
            unitOfWork.Wallet.AddRange<PaypallAcount>(paypallAccounts);
            unitOfWork.Wallet.AddRange<Models.CreditCard>(creditCardsData);
            unitOfWork.Wallet.AddRange<PersonalInfo>(personalInfos);
            unitOfWork.Wallet.AddRange<Models.Address>(addressData);




            unitOfWork.SaveChanges();






        }
        private static string CreatePasswordHash(string password, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            byte[] passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            return  Convert.ToBase64String(passwordHash);
        }
    }
}
