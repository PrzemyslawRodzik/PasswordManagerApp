using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PasswordManagerApp.ApiResponses;
using PasswordManagerApp.Models;
using PasswordManagerApp.RealTimeAlerts;
using PasswordManagerApp.Services;
using Quartz;



namespace PasswordManagerApp.ScheludeTasks.Jobs
{   

public class OldPasswordsCheckJob : IJob
    {
        private readonly NotificationService _notificationService;

        public OldPasswordsCheckJob(NotificationService notificationService)
        {

            _notificationService = notificationService;

        }
        public Task Execute(IJobExecutionContext context)
        {   
             string jobInstanceDescription =  context.JobDetail.Description;
             if(jobInstanceDescription.Equals("OldPasswordsCheckJobForLoggedUsers") )
             {
                _notificationService.InformUsersAboutOldPasswords();
                
             }
                
            if(jobInstanceDescription.Equals("OldPasswordsCheckJobForAll") )  
             {
                
                    
             }
            
         return Task.CompletedTask;
       }


       
    }
}