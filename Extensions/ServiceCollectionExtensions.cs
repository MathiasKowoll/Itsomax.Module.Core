using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Data.Infrastructure.Web.ModelBinders;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Itsomax.Module.Core.Extensions
{
    
    public static class ServiceCollectionExtensions
    {
        private static readonly IModuleConfigurationManager ModulesConfig = new ModuleConfigurationManager();
        public static IServiceCollection LoadInstalledModules(this IServiceCollection services, string contentRootPath)
        {
            var modules = new List<ModuleInfo>();
            var modulesFolder = Path.Combine(contentRootPath, "Modules");
            const string moduleManifestName = "module.json";
            //var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var module in ModulesConfig.GetModules())
            {   
                var moduleFolder = new DirectoryInfo(Path.Combine(modulesFolder, module.Id));
                var moduleManifestPath = Path.Combine(moduleFolder.FullName, moduleManifestName);
                
                if (!File.Exists(moduleManifestPath))
                {
                    throw new MissingModuleManifestException($"The manifest for the module '{moduleFolder.Name}' is not found.", moduleFolder.Name);
                }
                
                using (var reader = new StreamReader(moduleManifestPath))
                {
                    var content = reader.ReadToEnd();
                    dynamic moduleMetadata = JsonConvert.DeserializeObject(content);
                    module.Name = moduleMetadata.name;
                    module.IsBundledWithHost = moduleMetadata.isBundledWithHost;
                }

                
                if (!moduleFolder.Exists)
                {
                    continue;
                }

                foreach (var file in moduleFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException)
                    {
                        // Get loaded assembly
                        assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                        if (assembly == null)
                        {
                            throw;
                        }
                    }
                    
                    
                    if (assembly.FullName.Contains(module.Id))
                    {
                        modules.Add(new ModuleInfo
                        {
                            Id = module.Id,
                            Name = module.Name,
                            Assembly = assembly,
                            IsBundledWithHost = module.IsBundledWithHost
                            //Path = moduleFolder.FullName
                        });
                    }
                }
            }

            foreach (var module in modules)
            {
                var moduleInitializerType = module.Assembly.GetTypes()
                    .FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializerType);
                }
            }
            
            GlobalConfiguration.Modules = modules;
            return services;
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules)
        {
            var mvcBuilder = services
                .AddMvc(o =>
                {
                    o.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddNToastNotifyToastr()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            foreach (var module in modules)
            {
                AddApplicationPart(mvcBuilder.PartManager,module.Assembly);
            }

            return services;
        }
        
        private static void AddApplicationPart(ApplicationPartManager applicationPartManager, Assembly assembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var part in partFactory.GetApplicationParts(assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, false);
            foreach (var relatedAssembly in relatedAssemblies)
            {
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                foreach (var part in partFactory.GetApplicationParts(relatedAssembly))
                {
                    applicationPartManager.ApplicationParts.Add(part);
                }
            }
        }

        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(o =>
                {
                    o.Password.RequireDigit =
                        Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireDigit").Value);
                    o.Password.RequireNonAlphanumeric =
                        Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireNonAlphanumeric").Value);
                o.Password.RequiredLength = Convert.ToInt32(configuration.GetSection("ConfigSystem:RequiredLength").Value);
                    o.Password.RequireLowercase =
                        Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireLowercase").Value);
                    o.Password.RequireUppercase =
                        Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireUppercase").Value);
                    o.Lockout.AllowedForNewUsers =
                        Convert.ToBoolean(configuration.GetSection("ConfigSystem:AllowedForNewUsers").Value);
                    o.Lockout.MaxFailedAccessAttempts =
                        Convert.ToInt32(configuration.GetSection("ConfigSystem:MaxFailedAccessAttempts").Value);
                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(
                        Convert.ToInt32(configuration.GetSection("ConfigSystem:DefaultLockoutTimeSpanDays").Value));


                })
            .AddDefaultTokenProviders()
            .AddRoleStore<ItsomaxRoleStore>()
            .AddUserStore<ItsomaxUserStore>();
            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = Convert.ToString(configuration.GetSection("ConfigSystem:LoginPath").Value);
                o.LogoutPath = Convert.ToString(configuration.GetSection("ConfigSystem:LogoutPath").Value);
                o.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(configuration.GetSection("ConfigSystem:CookieExpireTimeSpanMin").Value));
                o.AccessDeniedPath = Convert.ToString(configuration.GetSection("ConfigSystem:AccessDeniedPath").Value);
            });

            return services;
        }

        public static IServiceProvider Build(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(RepositoryWithTypedId<,>)).As(typeof(IRepositoryWithTypedId<,>));

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterType<SequentialMediator>().As<IMediator>();
            
            foreach (var module in GlobalConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("ServiceProvider")).AsImplementedInterfaces();
                builder.RegisterAssemblyTypes(module.Assembly).Where(t => t.Name.EndsWith("Handler")).AsImplementedInterfaces();
            }

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(hostingEnvironment);
            builder.Populate(services);
            
            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }

        private static Task HandleRemoteLoginFailure(RemoteFailureContext ctx)
        {
            ctx.Response.Redirect("/Login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }

    }
}
