using Itsomax.Data.Infrastructure;
using Itsomax.Module.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Itsomax.Module.Core.Models
{
    public static class SeedInitialdata
    {
        private static readonly string[] AppSettingsListBool = new string[] { "SeedData", "NewModule", "CreateAdmin", "RefreshClaims", "NewModuleCreateMenu" };
        private static readonly string[] AppSettingsListEmpty = new string[] {"SmptUrl", "SmptAccount", "SmptPassword" };

        public static async Task CreateDB(IServiceProvider serviceProvider)
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

        public static void InitialAppSettings(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {

                foreach (var appSettingsListBool in AppSettingsListBool)
                {
                    if (!context.AppSettings.Any(x => x.Key == appSettingsListBool))
                    {
                        var appSave = new AppSetting { Key = appSettingsListBool, Value = "true" };
                        context.AppSettings.Add(appSave);
                        context.SaveChanges();
                    }
                }

                foreach (var appSettingsListEmpty in AppSettingsListEmpty)
                {
                    if (!context.AppSettings.Any(x => x.Key == appSettingsListEmpty))
                    {
                        var appSave = new AppSetting { Key = appSettingsListEmpty, Value = "" };
                        context.AppSettings.Add(appSave);
                        context.SaveChanges();
                    }
                }
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
                        /*
                        var subModDB = context.SubModule.ToList();
                        foreach (var item in subModDB)
                        {
                            var testsubmodDll = modContentDB.Distinct().ToList();
                            var submodDll = modContentDB.FirstOrDefault(x => x.Controller == item.Name);
                            if (submodDll == null)
                            {
                                var sub = context.SubModule.FirstOrDefault(x => x.Id == item.Id);
                                context.SubModule.Remove(sub);
                                context.SaveChanges();
                            }
                        }
                        */

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

                var newModule = context.AppSettings.FirstOrDefault(x => x.Key == "NewModule");
                newModule.Value = "false";
                context.Entry(newModule).State= EntityState.Modified;
                context.SaveChanges();
                var newMenu = context.AppSettings.FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
                newMenu.Value = "true";
                context.Entry(newMenu).State = EntityState.Modified;
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
                        createAdmin.Value = "false";
                        context.Entry(createAdmin).State = EntityState.Modified;
                        context.SaveChanges();
                        return;
                    }

                }
                else
                {
                    var passwordToken = userManager.GeneratePasswordResetTokenAsync(userExist).Result;
                    await userManager.ResetPasswordAsync(userExist, passwordToken, "Admin123.,");
                    var createAdmin = context.AppSettings.FirstOrDefault(x => x.Key == "CreateAdmin");
                    createAdmin.Value = "false";
                    context.Entry(createAdmin).State = EntityState.Modified;
                    context.SaveChanges();
                    return;
                }

            }

        }
    }
}
