using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;
using System.IO;
using Itsomax.Module.Core.Interfaces;
using System.Linq;
using System;

namespace Itsomax.Module.Core.Services
{
    public class CreateMenu :ICreateMenu
    {
        private readonly IRepository<Modules> _module;
        private readonly IManageFiles _manageFile;
        private readonly IRepository<AppSetting> _appSettings;
        public CreateMenu(IRepository<Modules> module, IRepository<AppSetting> appSettings,
                          IManageFiles manageFile)
        {
            _module = module;
            _manageFile = manageFile;
            _appSettings = appSettings;
        }

        public void CreteMenuFile()
        {
            var filePath = Path.Combine(GlobalConfiguration.ContentRootPath, "Views", "Shared");
            var file = "_AdminSideMenu.cshtml";
            var modules = _module.Query().Where(x => x.IsValidModule == true && x.ShortName.Contains("Management")).ToList();
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
                        sidebarMenu = sidebarMenu + "@await Html.PartialAsync(\"" + checkFile + "\")" + Environment.NewLine;
                    }
                }
                count++;
            }
            _manageFile.EditFile(filePath, sidebarMenu, file);
            var appSettings = _appSettings.Query().FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
            if (appSettings != null) appSettings.Value = "false";
            _appSettings.SaveChanges();

        }
    }
}