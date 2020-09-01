using System;
using EmailService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordManagerApp.Extensions;
using PasswordManagerApp.Models;
using PasswordManagerApp.Repositories;
using PasswordManagerApp.Services;


namespace PasswordManagerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddDataProtection();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.Cookie.Name = "UserCookie";
                    options.LoginPath = "/auth/login";



                });




            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserService, UserService>();

        /*  MySql Database
            services.AddDbContext<ApplicationDbContext>(options =>
                       options.UseMySql(Configuration.GetConnectionString("mysqlconnection")));

         */


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));




            services.ConfigureRepositoryWrapper();
          
            /* services.AddAuthorization(options =>
            {
                options.AddPolicy("TwoFactorPolicy", policy =>
                                  policy.RequireClaim("TwoFactorAuth", "1"));
            });

            */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext applicationDbContext)
        {

            applicationDbContext.Database.Migrate();
            
           // DataSeeder.SeedData(applicationDbContext);
            if (env.IsDevelopment())
            {   
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
