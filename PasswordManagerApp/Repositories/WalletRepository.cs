using Microsoft.EntityFrameworkCore;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public class WalletRepository : RepositoryBase, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public IEnumerable<LoginData> GetLoginDataBreach() 
        {
            try
            {
                return ApplicationDbContext.LoginDatas.Where(ld => ld.Compromised == 1).ToList();
            }
            catch (ArgumentNullException )
            {
                return null;
            }
            
        }
        public IEnumerable<PaypallAcount> GetPaypallBreach()
        {

            try
            {
                return ApplicationDbContext.PaypallAcounts.Where(ld => ld.Compromised == 1).ToList();
            }
            catch (ArgumentNullException)
            {
                return null;
            }



          
        }
        



    }
}
