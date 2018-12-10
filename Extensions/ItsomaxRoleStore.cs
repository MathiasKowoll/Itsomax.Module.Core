//using System.Security.Claims;

using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Itsomax.Module.Core.Extensions
{
    public class ItsomaxRoleStore : RoleStore<Role, ItsomaxDbContext, long, UserRole, IdentityRoleClaim<long>>
    {
        public ItsomaxRoleStore(ItsomaxDbContext context) : base(context)
        {
        }
    }
}