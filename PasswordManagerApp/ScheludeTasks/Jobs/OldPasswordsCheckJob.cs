using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.Services;
using Quartz;



namespace PasswordManagerApp.ScheludeTasks.Jobs
{   

public class OldPasswordsCheckJob : IJob
    {
        
        private readonly IUserService _userService;
        
        private readonly ILogger<OldPasswordsCheckJob> _logger;

    
        public OldPasswordsCheckJob(IUserService userService, ILogger<OldPasswordsCheckJob> logger )
        {
             
            _userService = userService;
            _logger = logger;
            
        }
        

                
       private void InformUsers()
       {
           var userId =  _userService.GetAuthUserId();
                if(userId == -1)
                    return;
                _userService.InformUserAboutOldPasswords(userId);
       }
            
       
        public Task Execute(IJobExecutionContext context)
        {
             string jobInstanceDescription =  context.JobDetail.Description;
             if(jobInstanceDescription.Equals("OldPasswordsCheckJobForSpecificUser") )
             {   
                 InformUsers();
                 _logger.LogInformation(jobInstanceDescription);
             }
                
            if(jobInstanceDescription.Equals("OldPasswordsCheckJobForAll") )  
             {
                 _userService.InformAllUsersAboutOldPasswords();
                    _logger.LogInformation(jobInstanceDescription);
             }
            

         return Task.CompletedTask;
       
           

        }

        
    }
}