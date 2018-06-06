using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AD.AuthService.IdentityServer4;
using AD.IdentityModels;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql;

namespace AD.AuthService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IdentityServerConfig _identityServerConfig;

        public Startup(IHostingEnvironment env)
        {
            var envName = env.EnvironmentName;
            var settingsFile = envName == "" ? "appsettings.json" : $"appsettings.{envName}.json";
            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFile, optional : true, reloadOnChange : true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();

            Configuration = builder.Build();

            _identityServerConfig = new IdentityServerConfig(Configuration);
        }

        string BuildConnectionString()
        {
            var connString = Configuration.GetConnectionString("DefaultConnection");
            const string dbPasswordKey = "MySqlAdminPassword";
            connString = connString.Replace($"@@{dbPasswordKey}@@",
                Configuration[dbPasswordKey]);

            return connString;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ASP.NET Identity services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(BuildConnectionString()));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // IdentityServer4 services
            //services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            //services.AddTransient<IProfileService, IdentityWithClaimsProfileService>();

            // configure IdentityServer4 with in-memory stores, keys, clients and resources
            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2(Configuration.GetSection("Tokens") ["SigningCertificateFile"]))
                //.AddTemporarySigningCredential()
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiResources(_identityServerConfig.GetApiResources())
                .AddInMemoryClients(_identityServerConfig.GetClients());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors(cfg =>
            {
                cfg.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            app
                .UseAuthentication()
                .UseIdentityServer(); // Note: UseIdentityServer() must come after UseIdentity()
        }
    }
}