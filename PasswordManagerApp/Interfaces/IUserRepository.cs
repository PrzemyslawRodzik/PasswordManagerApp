using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public interface IUserRepository :IRepositoryBase
    {
        IEnumerable<User> GetMethodFromIUserRepository();
        IEnumerable<User> Get2MethodFromIUserRepository();
    }
}
