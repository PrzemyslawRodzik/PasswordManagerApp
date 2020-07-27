using PasswordManagerApp.Models.Entities;
using PasswordManagerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Interfaces
{
    public interface IWalletRepository: IRepositoryBase
    {
        IEnumerable<LoginData> GetLoginDataBreach();
        IEnumerable<PaypallAcount> GetPaypallBreach();
    }
}
