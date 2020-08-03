using PasswordManagerApp.Models;
using PasswordManagerApp.Repositories;
using System.Collections.Generic;


namespace PasswordManagerApp.Interfaces
{
    public interface IWalletRepository: IRepositoryBase
    {
        IEnumerable<LoginData> GetAllLoginDataBreach();
        IEnumerable<PaypallAcount> GetAllPaypallBreach();
        int GetDataCountForUser<TEntity>(User user) where TEntity : UserRelationshipModel;
        int GetDataBreachForUser<TEntity>(User user) where TEntity : class, ICompromisedEntity;
    }
}
