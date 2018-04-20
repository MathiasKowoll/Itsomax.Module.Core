using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Itsomax.Data.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Itsomax.Module.Core.Extensions;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;
using Itsomax.Module.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace Itsomax.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

        }
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<SignInManager<User>, ItsomaxSignInManager<User>>();
            serviceCollection.AddSingleton<ICreateMenu, CreateMenu>();
            serviceCollection.AddSingleton<IEmailService, EmailService>();
            serviceCollection.AddSingleton<IGetRemoteInformation, GetRemoteInformation>();
            serviceCollection.AddSingleton<ILogginToDatabase, LogginToDatabase>();
            serviceCollection.AddSingleton<IManageExcelFile, ManageExcelFile>();
            serviceCollection.AddSingleton<IManageFiles, ManageFiles>();
            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy("ManageAuthentification",
                    policy => policy.Requirements.Add(new ManageAuthentificationRequirement()));
            });
            serviceCollection.AddSingleton<IAuthorizationHandler,ManageAuthentificationHandler>();
        }
    }
}