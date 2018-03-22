using System.Collections.Generic;
using Itsomax.Data.Infrastructure.Models;
namespace Itsomax.Module.Core.Models
{
    public class SubModule : EntityBase
    {
        public string Name { get; set; }
        public long ModulesId { get; set; }
        public Modules Modules { get; set; }
        public IList<ModuleRole> ModuleRoles { get; set; } = new List<ModuleRole>();
        
    }
}