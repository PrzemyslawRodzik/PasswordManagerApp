using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using PasswordManagerApp.Handlers;

using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Quartz;



namespace PasswordManagerApp.ScheludeTasks.Jobs
{   
 [DisallowConcurrentExecution]   
public class PasswordBreachCheckJob : IJob
    {
        
        private readonly IUserService _userService;
        
        //private readonly IEmailSender _emailSender;
        
        

    
        public PasswordBreachCheckJob(IUserService userService)
        {
            _userService = userService;
        }   
           
        
        private void CheckMasterPasswordBreach()
        {
              /*  var authUserId = _userService.GetAuthUserId();
                if(authUserId == -1)
                    return;
                
                string password = _unitOfWork.Users.Find<User>(authUserId).Password;
                var isPwned = PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result;
            */
               
        }
        public Task Execute(IJobExecutionContext context)
        {    
                
            //CheckMasterPasswordBreach();
            


            return Task.CompletedTask;

        }
    }
}