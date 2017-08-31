using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;
using Microsoft.AspNetCore.Authorization;
using NToastNotify;
using Itsomax.Data.Infrastructure.Web.ModelBinders;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
//using Hangfire;

namespace Itsomax.Module.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadInstalledModules(this IServiceCollection services, string contentRootPath)
        {
            var modules = new List<ModuleInfo>();
            var moduleRootFolder = new DirectoryInfo(Path.Combine(contentRootPath, "Modules"));
            var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var moduleFolder in moduleFolders)
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (!binFolder.Exists)
                {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
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

                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        modules.Add(new ModuleInfo
                        {
                            Name = moduleFolder.Name,
                            Assembly = assembly,
                            Path = moduleFolder.FullName
                        });
                    }
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
                .AddRazorOptions(o =>
                {
                    foreach (var module in modules)
                    {
                        o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    }
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            foreach (var module in modules)
            {
                // Register controller from modules
                mvcBuilder.AddApplicationPart(module.Assembly);

                // Register dependency in modules
                var moduleInitializerType =
                    module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    moduleInitializer.Init(services);
                }
            }

            return services;
        }

        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services, IConfigurationRoot configuration)
        {
            //var configSystem = configuration.GetSection("UseSystem:DefaultApp").Value;
            //if(configSystem=="ItsomaxCMS")
            //{
            services.AddIdentity<User, Role>(o =>
            {
                o.Password.RequireDigit = Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireDigit").Value);
                o.Password.RequireNonAlphanumeric = Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireNonAlphanumeric").Value);
                o.Password.RequiredLength = Convert.ToInt32(configuration.GetSection("ConfigSystem:RequiredLength").Value);
                o.Password.RequireLowercase = Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireLowercase").Value);
                o.Password.RequireUppercase = Convert.ToBoolean(configuration.GetSection("ConfigSystem:RequireUppercase").Value);
                o.Lockout.AllowedForNewUsers = Convert.ToBoolean(configuration.GetSection("ConfigSystem:AllowedForNewUsers").Value);
                o.Lockout.MaxFailedAccessAttempts = Convert.ToInt32(configuration.GetSection("ConfigSystem:MaxFailedAccessAttempts").Value);
                o.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromDays(Convert.ToInt32(configuration.GetSection("ConfigSystem:DefaultLockoutTimeSpanDays").Value));


            })
            .AddDefaultTokenProviders()
            .AddRoleStore<ItsomaxRoleStore>()
            .AddUserStore<ItsomaxUserStore>();
                services.ConfigureApplicationCookie(o =>
                {
                    o.LoginPath = Convert.ToString(configuration.GetSection("ConfigSystem:LoginPath").Value);
                    o.LogoutPath = Convert.ToString(configuration.GetSection("ConfigSystem:LogoutPath").Value);
                    o.ExpireTimeSpan = System.TimeSpan.FromMinutes(Convert.ToInt32(configuration.GetSection("ConfigSystem:CookieExpireTimeSpanMin").Value));
                    o.AccessDeniedPath = Convert.ToString(configuration.GetSection("ConfigSystem:AccessDeniedPath").Value);
                });

            return services;
        }
 
        public static IServiceCollection AddCustomizedDataStore(this IServiceCollection services, IConfigurationRoot configuration)
        {
			var configDb = configuration.GetSection("UseConnection:DefaultConnection").Value;
            
            if(configDb=="Postgres")
			{
				services.AddDbContext<ItsomaxDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Postgres"),              
					b => b.MigrationsAssembly("Itsomax.AppHost")));
			}
            
            if (configDb == "SqlServer"){
				services.AddDbContext<ItsomaxDbContext>(options =>
		        options.UseSqlServer(configuration.GetConnectionString("SqlServer"),
				    b => b.MigrationsAssembly("Itsomax.AppHost")));
				
			}
			return services;
            
            
        }
        
		public static IServiceCollection AddCustomizedAuthorization(this IServiceCollection services)
		{
			services.AddAuthorization(options =>
			{
				options.AddPolicy("ManageAuthentification",
				                  policy => policy.Requirements.Add(new ManageAuthentificationRequirement()));
			});
			                          

			return services;
		}
        


        public static IServiceProvider Build(this IServiceCollection services,
            IConfigurationRoot configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(RepositoryWithTypedId<,>)).As(typeof(IRepositoryWithTypedId<,>));

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterType<SequentialMediator>().As<IMediator>();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            foreach (var module in GlobalConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).AsImplementedInterfaces();
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
