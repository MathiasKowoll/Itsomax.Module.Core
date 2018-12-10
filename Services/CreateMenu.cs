using System;
using System.IO;
using System.Linq;
using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Services
{
    public class CreateMenu :ICreateMenu
    {
        private readonly IRepository<Modules> _module;
        private readonly IManageFiles _manageFile;
        private readonly ItsomaxDbContext _context;
        public CreateMenu(IRepository<Modules> module,ItsomaxDbContext context,
                          IManageFiles manageFile)
        {
            _module = module;
            _manageFile = manageFile;
            _context = context;
        }

        public void CreteMenuFile()
        {
            var appSettingsExists = _context.Set<AppSetting>().FirstOrDefault(x => x.Key == "SystemNewModuleCreateMenu" && x.Value =="true");
            if (appSettingsExists == null)
            {
                return;
            }
            var filePath = Path.Combine(GlobalConfiguration.ContentRootPath, "Views", "Shared");
            var file = "_AdminSideMenu.cshtml";
            var modules = _module.Query().Where(x => x.IsValidModule && x.ShortName.Contains("Management")).ToList();
            var countModules = modules.Count();
            string sidebarMenu = "";

            _manageFile.CleanFile(filePath,file);

            var count = 1;
            while (count <= countModules)
            {
                foreach (var itemMod in modules)
                {
                    var checkFile = count + "_" + itemMod.ShortName + "SideMenu.cshtml";
                    if (_manageFile.ExistFile(filePath, checkFile))
                    {
                        sidebarMenu = sidebarMenu + "@await Html.PartialAsync(\"" + checkFile + "\")" +
                                      Environment.NewLine;
                    }
                }
                count++;
            }
            _manageFile.EditFile(filePath, sidebarMenu, file);
            var appSettings = _context.Set<AppSetting>().FirstOrDefault(x => x.Key == "SystemNewModuleCreateMenu");
            if (appSettings != null) appSettings.Value = "false";
            _context.SaveChanges();

        }
    }
}