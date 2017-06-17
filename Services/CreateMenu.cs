using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;
using System.IO;
using Itsomax.Module.Core.Interfaces;
using System.Linq;
using Itsomax.Module.Core.Extensions.CommonHelpers;
namespace Itsomax.Module.Core.Services
{
    public class CreateMenu :ICreateMenu
    {
        private readonly IRepository<ModuleContent> _moduleContent;
        private readonly IRepository<Modules> _module;
        private readonly IManageFiles _manageFile;
        private readonly IRepository<SubModule> _subModule;
        public CreateMenu(IRepository<ModuleContent> moduleContent,IRepository<Modules> module,
                          IManageFiles manageFile,IRepository<SubModule> subModule)
        {
            _module = module;
            _moduleContent = moduleContent;
            _manageFile = manageFile;
            _subModule = subModule;
        }

        public void CreteMenuFile()
        {
            var filePath = Path.Combine(GlobalConfiguration.ContentRootPath, "Views", "Shared");
            var file = "_AdminSideMenu.cshtml";

            var modules = _module.Query().Where(x => x.isValid==true && x.ShortName.Contains("Management")).ToList();
            var subModules = _subModule.Query().ToList();

            string sidebarMenu = "";//"<li class=\"header\">MAIN NAVIGATION</li>";

            foreach (var itemMod in modules)
            {
                
                sidebarMenu = sidebarMenu +
                    "<li class=\"header\">"+StringHelperClass.CamelSplit(itemMod.ShortName).ToUpper() +"</li>";
                foreach(var itemSubMod in subModules)
                {
					sidebarMenu = sidebarMenu +
					"<li class=\"treeview\">" +
					"<a href =\"#\">" +
                    "<i class=\"fa fa-dashboard\"></i> <span>" + StringHelperClass.CamelSplit(itemSubMod.Name) + "</span>" +
					"<span class=\"pull-right-container\">" +
					"<i class=\"fa fa-angle-left pull-right\"></i>" +
					"</span>" +
					"</a>" +
					"<ul class=\"treeview-menu\">";
                    var modContent = _moduleContent.Query().Where(x => x.ModulesId == itemMod.Id && !x.Action.ToUpper().Contains("VIEW") && x.Controller == itemSubMod.Name).ToList();
					foreach (var itemCon in modContent)
					{
						sidebarMenu = sidebarMenu +
						"<li><a href=\"/" + itemCon.Controller + "/" + itemCon.Action + "\"><i class=\"fa fa-circle-o\"></i>" + StringHelperClass.CamelSplit(itemCon.Action) + "</a></li>";
					}
                    sidebarMenu = sidebarMenu +
                        "</ul>" +
                        "</li>";
                }
    
            }
            var content = _manageFile.GetFileContent(filePath, file);
            if(content!=sidebarMenu)
            {
                _manageFile.EditFile(filePath, sidebarMenu, file);
            }
            
        }
    }
}