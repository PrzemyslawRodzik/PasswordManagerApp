using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PasswordManagerApp.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _db;

        private IEnumerable<User> users;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<User> Create(string email, string password)
        {
            // test branches 1234

             
            return null;

        }

        public Task<User> AddExternal(string id, string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> Authenticate(string email, string password)
        {
            return null;
            
        }

        public Task<User> AuthenticateExternal(string id)
        {
            throw new NotImplementedException();
        }

      

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void delete(int id)
        {
            throw new NotImplementedException();
        }

         public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        User IUserService.Create(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
