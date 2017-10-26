using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;
using System.IO;
using Itsomax.Module.Core.Interfaces;
using System.Linq;
using Itsomax.Module.Core.Extensions.CommonHelpers;
using System;

namespace Itsomax.Module.Core.Services
{
    public class CreateMenu :ICreateMenu
    {
        private readonly IRepository<ModuleContent> _moduleContent;
        private readonly IRepository<Modules> _module;
        private readonly IManageFiles _manageFile;
        private readonly IRepository<SubModule> _subModule;
        private readonly IRepository<AppSetting> _appSettings;
        public CreateMenu(IRepository<ModuleContent> moduleContent,IRepository<Modules> module, IRepository<AppSetting> appSettings,
                          IManageFiles manageFile,IRepository<SubModule> subModule)
        {
            _module = module;
            _moduleContent = moduleContent;
            _manageFile = manageFile;
            _subModule = subModule;
            _appSettings = appSettings;
        }

        public void CreteMenuFile()
        {
            var filePath = Path.Combine(GlobalConfiguration.ContentRootPath, "Views", "Shared");
            var file = "_AdminSideMenu.cshtml";
            var modules = _module.Query().Where(x => x.isValid == true && x.ShortName.Contains("Management")).ToList();
            var countModules = modules.Count();
            string sidebarMenu = "";

            var count = 1;
            while (count <= countModules)
            {
                foreach (var itemMod in modules)
                {
                    var checkFile = count.ToString() + "_" + itemMod.ShortName + "SideMenu.cshtml";
                    if (_manageFile.ExistFile(filePath, checkFile))
                    {
                        sidebarMenu = sidebarMenu + "@await Html.PartialAsync(\"" + checkFile + "\")" + Environment.NewLine+ "<hr class=\"m-t-0 m-b-40\">" + Environment.NewLine;
                    }
                }
                count++;
            }
            _manageFile.EditFile(filePath, sidebarMenu, file);
            var appSettings = _appSettings.Query().FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
            appSettings.Value = "false";
            _appSettings.SaveChange();



            /*
            foreach (var itemMod in modules)
            {
                var count = 1;
                while (count <= countModules)
                {
                    var checkFile = count.ToString()+"_"+ itemMod.ShortName + "SideMenu.cshtml";
                    var exist = _manageFile.ExistFile(filePath, checkFile);
                    if(_manageFile.ExistFile(filePath, checkFile))
                    {
                        sidebarMenu = sidebarMenu + "@await Html.PartialAsync(\"" + checkFile + "\")"+ Environment.NewLine;
                    }
                    count++;
                }
            }
            _manageFile.EditFile(filePath, sidebarMenu, file);
            var appSettings = _appSettings.Query().FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
            appSettings.Value = "false";
            _appSettings.SaveChange();
            */

        }

        /*
        public void CreteMenuFile()
        {
            var filePath = Path.Combine(GlobalConfiguration.ContentRootPath, "Views", "Shared");
            var file = "_AdminSideMenu.cshtml";

            var modules = _module.Query().Where(x => x.isValid==true && x.ShortName.Contains("Management")).ToList();
            

            string sidebarMenu = "";

            foreach (var itemMod in modules)
            {

                sidebarMenu = sidebarMenu +
                    "@if ((User.HasClaim(c => c.Value.ToString()==\"HasAccess\" && (c.Type.Contains(\"User\") || c.Type.Contains(\"Role\"))) || User.IsInRole(\"Admin\")))" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "<li class=\"nav-small-cap\">" + StringHelperClass.CamelSplit(itemMod.ShortName).ToUpper() + "</li>" + Environment.NewLine +
                    "}"+Environment.NewLine;
                var subModules = _subModule.Query().Where(x => x.ModulesId==itemMod.Id).ToList();
                foreach (var itemSubMod in subModules)
                {
					sidebarMenu = sidebarMenu +
                    "@if ((User.HasClaim(c => c.Value.ToString()==\"HasAccess\" && (c.Type.Contains(\""+itemSubMod.Name+"\"))) || User.IsInRole(\"Admin\")))" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "<li>" + Environment.NewLine+

                    "<a href =\"#\" class=\"has-arrow waves-effect waves-dark\" aria-expanded=\"false\">" + Environment.NewLine+
                    "<i class=\"mdi mdi-gauge\"></i> <span class=\"hide-menu\">" + StringHelperClass.CamelSplit(itemSubMod.Name) + "</span>" + Environment.NewLine+

                    //"<span class=\"pull-right-container\">" + Environment.NewLine+

                    //"<i class=\"fa fa-angle-left pull-right\"></i>" + Environment.NewLine+

                    //"</span>" + Environment.NewLine +
                    "</a>" + Environment.NewLine +
                    "<ul aria-expanded=\"false\" class=\"collapse\">" + Environment.NewLine;
                    var modContent = _moduleContent.Query().Where(x => x.ModulesId == itemMod.Id && !x.Action.ToUpper().Contains("VIEW") && x.Controller == itemSubMod.Name).ToList();
					foreach (var itemCon in modContent)
					{
						sidebarMenu = sidebarMenu +
						"<li><a href=\"/" + itemCon.Controller + "/" + itemCon.Action + "\">" + StringHelperClass.CamelSplit(itemCon.Action) + "</a></li>"+ Environment.NewLine;
					}
                    sidebarMenu = sidebarMenu +
                        "</ul>" + Environment.NewLine +
                        "</li>" + Environment.NewLine +
                        "}" + Environment.NewLine;
                }

    
            }
            _manageFile.EditFile(filePath, sidebarMenu, file);
            var appSettings = _appSettings.Query().FirstOrDefault(x => x.Key == "NewModuleCreateMenu");
            appSettings.Value = "false";
            _appSettings.SaveChange();
        }
        */
    }
}