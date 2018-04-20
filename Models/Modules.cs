using System.Collections.Generic;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class Modules : EntityBase
    {
        public string Name { get; set; }
        public bool IsValidModule { get; set; }
        public string ShortName { get; set; }
        public string Path { get; set; }
        public IList<ModuleContent> ModuleContent { get; set; } = new List<ModuleContent>();
        public IList<SubModule> SubModules { get; set; } = new List<SubModule>();
    }
}