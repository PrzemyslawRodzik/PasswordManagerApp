using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using PasswordManagerApp.Handlers;

using PasswordManagerApp.Models;
using PasswordManagerApp.Services;
using Quartz;



namespace PasswordManagerApp.ScheludeTasks.Jobs
{   
  
public class PasswordBreachCheckJob : IJob
    {
        private readonly NotificationService _notificationService;

        public PasswordBreachCheckJob(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }   
           
        
        
        public Task Execute(IJobExecutionContext context)
        {

            _notificationService.InformUsersAboutPasswordsBreach();

            

            return Task.CompletedTask;

        }
    }
}