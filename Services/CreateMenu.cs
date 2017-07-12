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
            

            string sidebarMenu = "";//"<li class=\"header\">MAIN NAVIGATION</li>";

            foreach (var itemMod in modules)
            {

                sidebarMenu = sidebarMenu +
                    "@if ((User.HasClaim(c => c.Value.ToString()==\"HasAccess\" && (c.Type.Contains(\"User\") || c.Type.Contains(\"Role\"))) || User.IsInRole(\"Admin\")))" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "<li class=\"header\">" + StringHelperClass.CamelSplit(itemMod.ShortName).ToUpper() + "</li>" + Environment.NewLine +
                    "}"+Environment.NewLine;
                var subModules = _subModule.Query().Where(x => x.ModulesId==itemMod.Id).ToList();
                foreach (var itemSubMod in subModules)
                {
					sidebarMenu = sidebarMenu +
                    "@if ((User.HasClaim(c => c.Value.ToString()==\"HasAccess\" && (c.Type.Contains(\""+itemSubMod.Name+"\"))) || User.IsInRole(\"Admin\")))" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "<li class=\"treeview\">" + Environment.NewLine+

                    "<a href =\"#\">" + Environment.NewLine+
                    "<i class=\"fa fa-dashboard\"></i> <span>" + StringHelperClass.CamelSplit(itemSubMod.Name) + "</span>" + Environment.NewLine+

                    "<span class=\"pull-right-container\">" + Environment.NewLine+

                    "<i class=\"fa fa-angle-left pull-right\"></i>" + Environment.NewLine+

                    "</span>" + Environment.NewLine +
                    "</a>" + Environment.NewLine +
                    "<ul class=\"treeview-menu\">" + Environment.NewLine;
                    var modContent = _moduleContent.Query().Where(x => x.ModulesId == itemMod.Id && !x.Action.ToUpper().Contains("VIEW") && x.Controller == itemSubMod.Name).ToList();
					foreach (var itemCon in modContent)
					{
						sidebarMenu = sidebarMenu +
						"<li><a href=\"/" + itemCon.Controller + "/" + itemCon.Action + "\"><i class=\"fa fa-circle-o\"></i>" + StringHelperClass.CamelSplit(itemCon.Action) + "</a></li>"+ Environment.NewLine;
					}
                    sidebarMenu = sidebarMenu +
                        "</ul>" + Environment.NewLine +
                        "</li>" + Environment.NewLine +
                        "}" + Environment.NewLine;
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