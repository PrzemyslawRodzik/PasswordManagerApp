using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;


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

        public IEnumerable<LoginData> GetAllLoginDataBreach() 
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
        public IEnumerable<PaypallAcount> GetAllPaypallBreach()
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
        public IEnumerable<LoginData> GetUnchangedPasswordsForUser(int userId)
        {   
            var allloginDatasList = ApplicationDbContext.LoginDatas.Where(x=>x.UserId==userId).ToList();
            var loginDatasList = allloginDatasList.Where(x => (DateTime.UtcNow.ToLocalTime() - x.ModifiedDate).Days>=30 ).ToList();
            
            return loginDatasList;
        }
        public int  GetDataCountForUser<TEntity>(User user) where TEntity: UserRelationshipModel
        {
            Type type = typeof(TEntity);
            
                return ApplicationDbContext.Set<TEntity>().Where(ld => ld.User == user).ToList().Count();


        }
        public int GetDataBreachForUser<TEntity>(User user) where TEntity : class,ICompromisedEntity
        {
            

            return ApplicationDbContext.Set<TEntity>().Where(ld => ld.User == user && ld.Compromised==1).ToList().Count();


        }





    }
    public interface ICompromisedEntity
    {
        
        public int Compromised { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }


    }
    
}
