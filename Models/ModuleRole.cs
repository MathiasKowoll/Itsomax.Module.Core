using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class ModuleRole
    {
        public long SubModuleId { get; set; }
        public SubModule SubModule { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}