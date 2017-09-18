using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AD.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AD.IdentityManager
{
    internal class IdentityManager
    {
        private readonly ServiceCollection _services;
        public IServiceProvider ServiceProvider { get; }
        public IConfiguration Configuration { get; }

        public IdentityManager()
        {
            // set up the configuration
            var envName = "Development";
            var settingsFile = $"appsettings.{envName}.json";
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFile, optional: true, reloadOnChange: true)
                .AddUserSecrets<IdentityManager>()
                .Build();

            // create the service collection and register our types
            _services = new ServiceCollection();

            var connString = Configuration.GetConnectionString("DefaultConnection");
            const string dbPasswordKey = "MySqlAdminPassword";
            connString = connString.Replace($"@@{dbPasswordKey}@@",
                Configuration[dbPasswordKey]);

            _services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connString));

            _services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            _services.AddScoped<IUserService, UserService>();

            // build the IoC from the service collection
            ServiceProvider = _services.BuildServiceProvider();
        }

        private void EnsureDatabaseCreated()
        {
            var dbContext = ServiceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        private struct UserData
        {
            public string UserName;
            public string Password;
            public bool IsAdmin;
        }

        public async Task CreateUsers()
        {
            EnsureDatabaseCreated();

            var userService = ServiceProvider.GetService<IUserService>();
            var userList = new List<UserData>() {
                new UserData { UserName = "demo", Password = Configuration["DemoUserPassword"], IsAdmin = false },
                new UserData { UserName = "admin", Password = Configuration["AdminUserPassword"], IsAdmin = true },
            };

            foreach (var u in userList)
            {
                var result = await userService.CreateUser(u.UserName, u.Password, u.IsAdmin);
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Error creating user {u.UserName}: {result.ToString()}");
                }
            }
        }
    }

}