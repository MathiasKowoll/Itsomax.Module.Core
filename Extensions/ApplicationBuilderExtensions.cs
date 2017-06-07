using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using Itsomax.Data.Infrastructure;
//using Itsomax.Module.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;
//using Itsomax.Module.Web.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
//using Hangfire;

//using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomizedIdentity(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            return app;
        }

        public static IApplicationBuilder UseCustomizedMvc(this IApplicationBuilder app, IConfiguration config)
        {

            var route = config.GetSection("DefaultUrl:Url").Value;
            app.UseMvc(routes =>
			{
				routes.Routes.Add(new UrlSlugRoute(routes.DefaultHandler));

				routes.MapRoute(
					"default",
                    route);
                    //"{controller=Admin}/{action=Index}/{id?}");
			});
            return app;
        }

        public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (context) =>
                    {
                        var headers = context.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new CacheControlHeaderValue
                        {
                            NoCache = true,
                            NoStore = true,
                            MaxAge = TimeSpan.FromDays(-1)
                        };
                    }
                });
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (context) =>
                    {
                        var headers = context.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromDays(60)
                        };
                    }
                });
            }

            return app;
        }

        public static IApplicationBuilder UseCustomizedRequestLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("es-ES"),
                new CultureInfo("en-US"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("es-ES", "es-ES"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            return app;
        }

        /* 
        public static IApplicationBuilder UseCustomizeHttpContext(this IApplicationBuilder app)
        {
            RequestPath.Instance= new RequestPath (app.ApplicationServices.GetService<IHttpContextAccessor>());
            return app;
        }
*/
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var context = app.ApplicationServices.GetRequiredService<ItsomaxDbContext>())
            {

                var pendingMigrations = context.Database.GetPendingMigrations().Count();
                if (pendingMigrations >= 0)
                {
                    context.Database.Migrate();
                }



                var role = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (role == null)
                {
                    role = new Role
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    };
                    context.Roles.Add(role);
                    context.SaveChanges();
                }


                var user = context.Users.FirstOrDefault(x => x.UserName == "admin");
                if (user == null)
                {
                    user = new User
                    {
                        UserName = "admin",
                        Email = "admin@admin.com",
                        PasswordHash = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@ADMIN.COM",
                        NormalizedUserName = "ADMIN"
                    };
                    context.Users.Add(user);
                    context.SaveChanges();
                }


                var userRole = context.UserRoles.FirstOrDefault(x => x.RoleId == role.Id && x.UserId == user.Id);
                if (userRole == null)
                {
                    userRole = new UserRole
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    };
                    context.UserRoles.Add(userRole);
                    context.SaveChanges();
                }
                //TODO: Temporal solucion until I sort this out
                //Postgres
                var sql = "TRUNCATE TABLE \"public\".\"Core_ModuleContent\"";
                //SQL Server
                //var sql = "TRUNCATE TABLE Core_ModuleContent";
                context.Database.ExecuteSqlCommand(sql);
                //Postgres
                sql = "ALTER SEQUENCE \"public\".\"Core_ModuleContent_Id_seq\" restart;";
                //SQL Server
                //sql = "ALTER SEQUENCE \"public\".\"Core_ModuleContent_Id_seq\" restart;";
                context.Database.ExecuteSqlCommand(sql);
                foreach (var moduleConfig in GlobalConfiguration.Modules)
                {
                    var asm = moduleConfig.Assembly;
                    var projectName = moduleConfig.Name;
                    var modelContent = asm.GetTypes().
                    SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(d => d.ReturnType.Name.Contains("Result"))
                    .Select(n => new ModuleContent()
                    {
                        Controller = n.DeclaringType?.Name.Replace("Controller", ""),
                        Action = n.Name
                    });

                    var modules = context.Modules.FirstOrDefault(z => z.Name == moduleConfig.Name);
                    if (modules != null)
                    {
                        modules.Path = moduleConfig.Path;
                        context.Modules.Update(modules);
                        context.Entry(modules).State = EntityState.Modified;
                        context.SaveChanges();

                        foreach (var modConfig in modelContent)
                        {
                            var moduleContentAdd = new ModuleContent()
                                {
                                    Controller = modConfig.Controller,
                                    Action = modConfig.Action,
                                    ModulesId = modules.Id
                                };
                                context.ModuleContent.Add(moduleContentAdd);
                                context.SaveChanges();

                        }
                    }
                    else //If module does not exists
                    {
                        modules = new Modules
						{
							Name = moduleConfig.Name,
							ShortName = moduleConfig.ShortName,
							Path = moduleConfig.Path,
							isValid = true
						};
						context.Modules.Add(modules);
						context.SaveChanges();

                        foreach (var modConfig in modelContent)
                        {
                            var moduleContentAdd = new ModuleContent()
                                {
                                    Controller = modConfig.Controller,
                                    Action = modConfig.Action,
                                    ModulesId = modules.Id
                                };
                                context.ModuleContent.Add(moduleContentAdd);
                                context.SaveChanges();

                        }



                    }
                }
            }
            return app;
        }
    }
}