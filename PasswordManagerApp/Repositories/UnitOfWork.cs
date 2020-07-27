using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        public  ApplicationDbContext Context { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            Context = context;
            Users = new UserRepository(Context);
            Wallet = new WalletRepository(Context);
            
        }

        public IWalletRepository Wallet { get; private set; }
        public IUserRepository Users { get; private set; }


        


        

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

       
    }
}
