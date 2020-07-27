using PasswordManagerApp.Models;
using PasswordManagerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Interfaces
{
   public interface IUnitOfWork
    {
        
        IUserRepository Users { get; }
        IWalletRepository Wallet { get; }

        
        ApplicationDbContext Context { get; }

        
        int SaveChanges();
    }
}
