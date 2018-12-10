using System.Collections.Generic;
using Itsomax.Data.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace Itsomax.Module.Core.Models
{
    public class Role : IdentityRole<long>, IEntityWithTypedId<long>
    {
        public IList<UserRole> Users { get; set; } = new List<UserRole>(); 
        public IList<ModuleRole> ModuleRoles { get; set; } = new List<ModuleRole>();
    }
}