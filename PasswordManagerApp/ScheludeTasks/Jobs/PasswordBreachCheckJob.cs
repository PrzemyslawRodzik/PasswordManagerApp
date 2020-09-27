using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Quartz;



namespace PasswordManagerApp.ScheludeTasks.Jobs
{   
 [DisallowConcurrentExecution]   
public class PasswordBreachCheckJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        
        //private readonly IEmailSender _emailSender;
        
        

    
        public PasswordBreachCheckJob(IUnitOfWork unitOfWork,IUserService userService)
        {
            
            _unitOfWork = unitOfWork;
            _userService = userService;
            
           // _emailSender = emailSender;
        }
        private void CheckMasterPasswordBreach()
        {
                var authUserId = _userService.GetAuthUserId();
                if(authUserId == -1)
                    return;
                
                string password = _unitOfWork.Users.Find<User>(authUserId).Password;
                var isPwned = PwnedPasswords.IsPasswordPwnedAsync(password, new CancellationToken(), null).Result;
               // if (isPwned != -1)
                   // _logger.LogInformation("Jest skompromitowane ############################");
               // else
                    // _logger.LogInformation("Jest ok! @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        }
        public Task Execute(IJobExecutionContext context)
        {    
                
            //CheckMasterPasswordBreach();
            


            return Task.CompletedTask;

        }
    }
}