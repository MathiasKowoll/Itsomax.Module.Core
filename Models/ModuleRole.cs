using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class ModuleRole : EntityBase
    {
        public long ModulesId { get; set; }
        public Modules Modules { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}