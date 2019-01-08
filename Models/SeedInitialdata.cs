using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Itsomax.Data.Infrastructure;
using Itsomax.Module.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;

namespace Itsomax.Module.Core.Models
{
    public static class SeedInitialdata
    {
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

        public static void InitializeModules(IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                //Check Existance of modules, deactivate if does not exist on host.
                foreach (var item in context.Modules.ToList())
                {
                    var existModuleGlobal = GlobalConfiguration.Modules.FirstOrDefault(x => x.Id == item.Name);
                    if (existModuleGlobal == null)
                    {
                        var moduleDelete = context.Modules.FirstOrDefault(x => x.Id == item.Id);
                        if (moduleDelete == null) continue;
                        moduleDelete.IsValidModule = false;
                        context.Modules.Update(moduleDelete);
                        context.Entry(moduleDelete).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        var moduleActive = context.Modules.FirstOrDefault(x => x.Id == item.Id);
                        if(moduleActive == null) continue;
                        moduleActive.IsValidModule = true;
                        context.Modules.Update(moduleActive);
                        context.Entry(moduleActive).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                // Add new Modules and update existent modules
                foreach (var hostModule in GlobalConfiguration.Modules)
                {
                    var existModuleDb = context.Modules.FirstOrDefault(x => x.Name == hostModule.Id);
                    if (existModuleDb == null)
                    {
                        var module = new Modules
                        {
                            Name = hostModule.Id,
                            IsValidModule = true,
                            Path = String.Empty,
                            ShortName = hostModule.Name
                        };
                        context.Modules.Add(module);
                        context.SaveChanges();
                    }
                    else
                    {
                        existModuleDb.ShortName = hostModule.Name;
                        existModuleDb.IsValidModule = true;
                        context.Modules.Update(existModuleDb);
                        context.Entry(existModuleDb).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                //Create Submodules
                IList<ModuleContentViewModel> moduleContent = new List<ModuleContentViewModel>();
                foreach (var moduleConfig in GlobalConfiguration.Modules)
                {
                    var moduleId = context.Modules.FirstOrDefault(x => x.Name == moduleConfig.Id).Id;
                    var asm = moduleConfig.Assembly;
                    var modelContent = asm.GetTypes().SelectMany(t =>
                            t.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(d => d.ReturnType.Name.Contains("Result"))
                        .Select(n => new ModuleContentViewModel
                        {
                            Controller = n.DeclaringType?.Name.Replace("Controller", ""),
                            ModulesId = moduleId
                        });
                    foreach (var item in modelContent)
                    {
                        moduleContent.Add(item);
                    }

                }

                //get only controllers by module.
                var subModuleName = moduleContent.Where(x => x.Controller.Contains("Manage"))
                    .Select(x => new {x.ModulesId, x.Controller}).Distinct().ToList();

                foreach (var subModule in context.SubModule.ToList())
                {
                    var subModuleExist = subModuleName.FirstOrDefault(x =>
                        x.ModulesId == subModule.ModulesId && x.Controller == subModule.Name);
                    if (subModuleExist != null)
                    {
                        subModule.ActiveSubModule = true;
                        context.SubModule.Update(subModule);
                        context.Entry(subModule).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        subModule.ActiveSubModule = false;
                        context.SubModule.Update(subModule);
                        context.Entry(subModule).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }

                foreach (var hostSubModule in subModuleName)
                {
                    var subModuleDb = context.SubModule.FirstOrDefault(x =>
                        x.ModulesId == hostSubModule.ModulesId && x.Name == hostSubModule.Controller);
                    if (subModuleDb != null)
                    {
                        subModuleDb.ActiveSubModule = true;
                        context.SubModule.Update(subModuleDb);
                        context.Entry(subModuleDb).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        var newSubModule = new SubModule
                        {
                            Name = hostSubModule.Controller,
                            ModulesId = hostSubModule.ModulesId,
                            ActiveSubModule = true
                        };
                        context.SubModule.Add(newSubModule);
                        context.SaveChanges();
                    }
                }


                /*
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
                        modules.Path = String.Empty;
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
                            Name = moduleConfig.Id,
                            ShortName = moduleConfig.Name,
                            Path = String.Empty    ,
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

                var newModule = context.AppSettings.FirstOrDefault(x => x.Key == "SystemNewModule");
                if (newModule != null)
                {
                    newModule.Value = "false";
                    context.Entry(newModule).State = EntityState.Modified;
                }

                context.SaveChanges();
                var newMenu = context.AppSettings.FirstOrDefault(x => x.Key == "SystemNewModuleCreateMenu");
                if (newMenu != null)
                {
                    newMenu.Value = "true";
                    context.Entry(newMenu).State = EntityState.Modified;
                }

                context.SaveChanges();

            }
            */
            }
        }

        public static async Task CreateAdmin (IServiceProvider serviceProvider)
        {
            using (var context = new ItsomaxDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ItsomaxDbContext>>()))
            {
                if(context.AppSettings.Any(x => x.Key == "SystemCreateAdmin" && x.Value =="false"))
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
                    var createAdmin = context.AppSettings.FirstOrDefault(x => x.Key == "SystemCreateAdmin");
                    if (createAdmin != null)
                    {
                        createAdmin.Value = "false";
                        context.Entry(createAdmin).State = EntityState.Modified;
                    }

                    context.SaveChanges();
                }

            }

        }
    }
}
