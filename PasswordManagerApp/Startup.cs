using System;
using System.Net.Http;
using EmailService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PasswordManagerApp.Extensions;
using PasswordManagerApp.Handlers;
using PasswordManagerApp.Models;
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
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
            
            
            
            .AddRazorRuntimeCompilation();
              services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                  .AddCookie(options =>
                  {
                      options.LoginPath = "/auth/login";
                      options.AccessDeniedPath = "/auth/accessdenied";
                      options.ExpireTimeSpan=TimeSpan.FromMinutes(15);
                      options.SlidingExpiration=false;
                  
                  });





          /*  services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
            }); */
            services.AddHttpClient<ApiService>(c =>
            {
                c.BaseAddress = new Uri("https://localhost:44324/api/");
                
                

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
            });






            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<JwtHelper>();

        /*  MySql Database
            services.AddDbContext<ApplicationDbContext>(options =>
                       options.UseMySql(Configuration.GetConnectionString("mysqlconnection")));

         */


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));




            services.ConfigureRepositoryWrapper();

            services.ConfigureScheduleTasks();

            
          
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


           
            //applicationDbContext.Database.Migrate();

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
