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
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

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

        public static IApplicationBuilder SeedData(this IApplicationBuilder app, IConfiguration config)
        {
            using (var context = app.ApplicationServices.GetRequiredService<ItsomaxDbContext>())
            {
                var dataBaseExists = context.Database.GetAppliedMigrations().Count();
                if (dataBaseExists > 0)
                {
                    //Default Settings
                    var appSettings = context.AppSettings.ToList().Count();
                    if (appSettings==0)
                    {
                        var appSettingsAdd = new []
                        {
                            new AppSetting {Key = "NewInstallation", Value = "true"},
                            new AppSetting {Key = "CreateAdmin", Value = "true"},
                            new AppSetting {Key = "RefreshClaims", Value = "true"},
                            new AppSetting {Key = "SmptUrl", Value = ""},
                            new AppSetting {Key = "SmptAccount", Value = ""},
                            new AppSetting {Key = "SmptPassword", Value = ""},
                            new AppSetting {Key = "AdminMessage", Value =config.GetSection("WelcomePage:Message").Value}
                            /*
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            new AppSetting {Value = "SmptAdress", Key = ""},
                            */
                        };
                        context.AppSettings.AddRange(appSettingsAdd);
                        context.SaveChanges();
                    }


                    var modulesDB = context.Modules.ToList();
                    foreach (var item in modulesDB)
                    {

                        var existModuleGlobal = GlobalConfiguration.Modules.FirstOrDefault(x => x.Name == item.Name);
                        if (existModuleGlobal == null)
                        {
                            var moduleDelete = context.Modules.FirstOrDefault(x => x.Id == item.Id);
                            context.Modules.Remove(moduleDelete);
                            context.SaveChanges();
                        }
                    }
                    foreach (var moduleConfig in GlobalConfiguration.Modules)
                    {
                        var asm = moduleConfig.Assembly;
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

                            var modContentDB = context.ModuleContent.Where(x => x.ModulesId == modules.Id).ToList();
                            foreach (var item in modContentDB)
                            {
                                var modContentAsm = modelContent.FirstOrDefault(x => x.Controller == item.Controller && x.Action == item.Action);
                                if (modContentAsm == null)
                                {
                                    var modContentDel = context.ModuleContent.FirstOrDefault(x => x.Id == item.Id);
                                    context.ModuleContent.Remove(modContentDel);
                                    context.SaveChanges();
                                }

                            }

                            foreach (var modConfig in modelContent)
                            {
                                var modContent = context.ModuleContent.FirstOrDefault(x => x.Controller == modConfig.Controller && x.Action == modConfig.Action);
                                if (modContent == null)
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
                                else
                                {
                                    modContent.Controller = modConfig.Controller;
                                    modContent.Action = modConfig.Action;
                                    modContent.ModulesId = modules.Id;
                                    context.Entry(modContent).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                            var subModDB = context.SubModule.ToList();
                            foreach (var item in subModDB)
                            {
                                var submodDll = modContentDB.FirstOrDefault(x => x.Controller == item.Name);
                                if (submodDll == null)
                                {
                                    var sub = context.SubModule.FirstOrDefault(x => x.Id == item.Id);
                                    context.SubModule.Remove(sub);
                                    context.SaveChanges();
                                }
                            }

                        }
                        else //If module does not exists in Database
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

                    var subModules = context.ModuleContent.Where(x => x.Controller.Contains("Manage")).Select(x => new { x.ModulesId, x.Controller }).Distinct().ToList();
                    var subModulesDB = context.SubModule.ToList();

                    foreach (var item in subModulesDB)
                    {
                        var existDbNotinMod = subModules.FirstOrDefault(x => x.Controller == item.Name);
                        if (existDbNotinMod == null)
                        {
                            var subModRemove = context.SubModule.FirstOrDefault(x => x.Name == item.Name);
                            context.SubModule.Remove(subModRemove);
                            context.SaveChanges();
                        }
                    }
                    var subModulesAdd = context.SubModule.ToList();
                    foreach (var item in subModules)
                    {
                        var newSubModule = subModulesAdd.FirstOrDefault(x => x.Name == item.Controller);
                        if (newSubModule == null)
                        {
                            var subModAdd = new SubModule()
                            {
                                ModulesId = subModules.FirstOrDefault(x => x.Controller == item.Controller).ModulesId,
                                Name = item.Controller
                            };
                            context.SubModule.Add(subModAdd);
                            context.SaveChanges();
                        }
                    }
                }

                //var pendingMigrations = context.Database.GetPendingMigrations().Count();
                //if (pendingMigrations > 0)
                //{
                //  context.Database.Migrate();
                //}


            }
            return app;
        }
    }
}