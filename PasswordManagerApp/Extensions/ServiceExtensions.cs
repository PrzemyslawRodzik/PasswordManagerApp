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
              
            services.AddSingleton(
                new List<JobMetadata>(){
                                        new JobMetadata(Guid.NewGuid(), typeof(OldPasswordsCheckJob), "OldPasswordsCheckJobForAll", "0 0 0 5 * ? *"), // co miesiac o polnocy 5 dnia miesiaca
                                        new JobMetadata(Guid.NewGuid(), typeof(OldPasswordsCheckJob), "OldPasswordsCheckJobForSpecificUser", "0 0 5/3 ? * * *") // co 3 godziny zaczynając od 5 rano
                                    }.AsEnumerable()
                );
              
            services.AddHostedService<QuartzHostedService>();
        }
    }
}
