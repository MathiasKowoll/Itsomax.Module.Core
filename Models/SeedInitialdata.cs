using Itsomax.Data.Infrastructure;
using Itsomax.Module.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Itsomax.Module.Core.Models
{
    public static class SeedInitialdata
    {
        private static readonly string[] AppSettingsListBool = { "SeedData", "NewModule", "CreateAdmin", 
            "RefreshClaims", "NewModuleCreateMenu" };

        private static readonly string[] AppSettingsListEmpty =
            {"SystemTitle", "SystemLoginText", "SystemLoginImageUrl", "SystemBigLogoUrl","SystemSmallLogoUrl"};

        public static async Task CreateDb(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext (
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                if(context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }

        public static async Task CreateExtention(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText =
                        "SELECT * FROM pg_available_extensions where name like 'plpython2u' and installed_version is null";
                    await context.Database.OpenConnectionAsync();

                    using (var result =  command.ExecuteReaderAsync().Result)
                    {
                        if (result.HasRows)
                        {
                            context.Database.CloseConnection();
                            await context.Database.ExecuteSqlCommandAsync("CREATE EXTENSION plpython2u");
                        }
                        else
                        {
                            context.Database.CloseConnection();
                        }
                            
                    }
                }

                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText =
                        "SELECT * FROM pg_available_extensions where name like 'pgcrypto' and installed_version is null";
                    await context.Database.OpenConnectionAsync();

                    using (var result = command.ExecuteReaderAsync().Result)
                    {
                        if (result.HasRows)
                        {
                            context.Database.CloseConnection();
                            await context.Database.ExecuteSqlCommandAsync("CREATE EXTENSION pgcrypto");
                        }
                        else
                        {
                            context.Database.CloseConnection();
                        }
                    }
                }
            }
        }

        public static void InitialAppSettings(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                IList<AppSetting> appSettings = new List<AppSetting>();
                IList<AppSetting> appSettingsAllSettings = new List<AppSetting>();

                foreach (var appSettingsListBool in AppSettingsListBool)
                {
                    appSettingsAllSettings.Add(new AppSetting { Key = appSettingsListBool, Value = "true" });
                    if (context.AppSettings.Any(x => x.Key == appSettingsListBool)) continue;
                    appSettings.Add(new AppSetting { Key = appSettingsListBool, Value = "true" });
                }
                
                
                foreach (var appSettingsListEmpty in AppSettingsListEmpty)
                {
                    appSettingsAllSettings.Add(new AppSetting { Key = appSettingsListEmpty, Value = "" });
                    if (context.AppSettings.Any(x => x.Key == appSettingsListEmpty)) continue;
                    appSettings.Add(new AppSetting { Key = appSettingsListEmpty, Value = "" });
                }
               
                var settings = context.AppSettings.ToList();
                IList<AppSetting> appSettingsRemove = new List<AppSetting>();
                foreach (var item in settings)
                {
                    if (!appSettingsAllSettings.Any(x => x.Key.Equals(item.Key)))
                    {
                        appSettingsRemove.Add(item);
                    }
                }
                context.AppSettings.AddRange(appSettings);
                context.SaveChanges();
                context.AppSettings.RemoveRange(appSettingsRemove);
                context.SaveChanges();
            }
        }

        public static void InitializeModules(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                if(context.AppSettings.Any(x => x.Key == "NewModule" && x.Value == "false"))
                {
                    return;
                }

                var modulesDb = context.Modules.ToList();
                foreach (var item in modulesDb)
                {

                    var existModuleGlobal = GlobalConfiguration.Modules.FirstOrDefault(x => x.Name == item.Name);
                    if (existModuleGlobal != null) continue;
                    {
                        var moduleDelete = context.Modules.FirstOrDefault(x => x.Id == item.Id);
                        if (moduleDelete != null) context.Modules.Remove(moduleDelete);
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

                    var modConfigs = modelContent as ModuleContent[] ?? modelContent.ToArray();
                    var moduleContents = modelContent as ModuleContent[] ?? modConfigs.ToArray();
                    if (modules != null)
                    {
                        modules.Path = moduleConfig.Path;
                        context.Modules.Update(modules);
                        context.Entry(modules).State = EntityState.Modified;
                        context.SaveChanges();

                        var modContentDb = context.ModuleContent.Where(x => x.ModulesId == modules.Id).ToList();
                        foreach (var item in modContentDb)
                        {
                            var modContentAsm = moduleContents.FirstOrDefault(x => x.Controller == item.Controller && x.Action == item.Action);
                            if (modContentAsm != null) continue;
                            {
                                var modContentDel = context.ModuleContent.FirstOrDefault(x => x.Id == item.Id);
                                if (modContentDel != null) context.ModuleContent.Remove(modContentDel);
                                context.SaveChanges();
                            }

                        }

                        foreach (var modConfig in moduleContents)
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

                    }
                    else //If module does not exists in Database
                    {
                        modules = new Modules
                        {
                            Name = moduleConfig.Name,
                            ShortName = moduleConfig.ShortName,
                            Path = moduleConfig.Path,
                            IsValidModule = true
                        };
                        context.Modules.Add(modules);
                        context.SaveChanges();

                        foreach (var modConfig in modConfigs)
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
                var subModulesDb = context.SubModule.ToList();

                foreach (var item in subModulesDb)
                {
                    var existDbNotinMod = subModules.FirstOrDefault(x => x.Controller == item.Name);
                    if (existDbNotinMod == null)
                    {
                        var subModRemove = context.SubModule.FirstOrDefault(x => x.Name == item.Name);
                        if (subModRemove != null) context.SubModule.Remove(subModRemove);
                        context.SaveChanges();
                    }
                }
                var subModulesAdd = context.SubModule.ToList();
                foreach (var item in subModules)
                {
                    var newSubModule = subModulesAdd.FirstOrDefault(x => x.Name == item.Controller);
                    if (newSubModule == null)
                    {
                        var moduleId = subModules.FirstOrDefault(x => x.Controller == item.Controller);
                        if (moduleId != null)
                        {
                            var subModAdd = new SubModule()
                            {
                                ModulesId = moduleId.ModulesId,
                                Name = item.Controller
                            };
                            context.SubModule.Add(subModAdd);
                            context.SaveChanges();
                        }
                        
                    }
                }

                var newModule = context.AppSettings.FirstOrDefault(x => x.Key == "NewModule");
                if (newModule != null)
                {
                    newModule.Value = "false";
                    context.Entry(newModule).State = EntityState.Modified;
                }

                context.SaveChanges();
                var newMenu = context.AppSettings.FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
                if (newMenu != null)
                {
                    newMenu.Value = "true";
                    context.Entry(newMenu).State = EntityState.Modified;
                }

                context.SaveChanges();

            }
        }

        public static async Task CreateAdmin (IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                if(context.AppSettings.Any(x => x.Key == "CreateAdmin" && x.Value =="false"))
                {
                    return;
                }

                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new Role() { Name = "Admin" });

                }

                var userExist = await userManager.FindByNameAsync("admin");
                if (userExist is null)
                {
                    var user = new User()
                    {
                        UserName = "admin",
                        Email = "admin@admin.cl",
                        LockoutEnabled = false
                    };

                    var res = await userManager.CreateAsync(user, "Admin123.,");
                    if (res.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                        var createAdmin = context.AppSettings.FirstOrDefault(x => x.Key == "CreateAdmin");
                        if (createAdmin != null)
                        {
                            createAdmin.Value = "false";
                            context.Entry(createAdmin).State = EntityState.Modified;
                        }

                        context.SaveChanges();
                    }

                }
                else
                {
                    var passwordToken = userManager.GeneratePasswordResetTokenAsync(userExist).Result;
                    await userManager.ResetPasswordAsync(userExist, passwordToken, "Admin123.,");
                    var createAdmin = context.AppSettings.FirstOrDefault(x => x.Key == "CreateAdmin");
                    if (createAdmin != null)
                    {
                        createAdmin.Value = "false";
                        context.Entry(createAdmin).State = EntityState.Modified;
                    }

                    context.SaveChanges();
                }

            }

        }
        //TODO: create loading scripts for modules.
        /*
        public static void LoadInitialScript(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                //context.Database
            }
        }
        */
    }
}
