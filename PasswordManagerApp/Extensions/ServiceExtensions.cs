using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordManagerApp.ScheludeTasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordManagerApp.ScheludeTasks.Jobs;
using Microsoft.AspNetCore.Http;

namespace PasswordManagerApp.Extensions
{
    public static class ServiceExtensions
    {
        
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            // TO DO
        }

        
        public static void ConfigureScheduleTasks(this IServiceCollection services)
        {

            
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<QuartzJobRunner>();
            
            services.AddScoped<OldPasswordsCheckJob>();
            services.AddScoped<PasswordBreachCheckJob>();
              
           /* services.AddSingleton(
                new List<JobMetadata>(){
                  new JobMetadata(Guid.NewGuid(), typeof(OldPasswordsCheckJob), "OldPasswordsCheckJob", "0 0 12 1/1 * ? *"), // codziennie o 12:00  
                  new JobMetadata(Guid.NewGuid(), typeof(PasswordBreachCheckJob), "PasswordBreachCheckJob", "0 0 0/3 1/1 * ? *") // co 3 godziny
                  
                }.AsEnumerable()
                );
            */
           
            
            //services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(PasswordBreachCheckJob), "PasswordBreachCheckForLoggedUsers", "0 0/3 * 1/1 * ? *"));


            services.AddHostedService<QuartzHostedService>();
        }
    }
}
