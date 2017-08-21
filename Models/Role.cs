using Microsoft.AspNetCore.Identity;
using Itsomax.Data.Infrastructure.Models;
using System.Collections.Generic;

namespace Itsomax.Module.Core.Models
{
    public class Role : IdentityRole<long>, IEntityWithTypedId<long>
    {
        public IList<UserRole> Users { get; set; } = new List<UserRole>(); 
    }
}