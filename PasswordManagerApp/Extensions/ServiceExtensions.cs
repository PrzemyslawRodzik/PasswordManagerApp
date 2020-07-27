using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PasswordManagerApp.Interfaces;
using PasswordManagerApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordManagerApp.Extensions
{
    public static class ServiceExtensions
    {
        

        

        

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            // TO DO
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
