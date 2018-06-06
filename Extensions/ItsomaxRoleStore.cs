//using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Extensions
{
    public class ItsomaxRoleStore : RoleStore<Role, ItsomaxDbContext, long, UserRole, IdentityRoleClaim<long>>
    {
        public ItsomaxRoleStore(ItsomaxDbContext context) : base(context)
        {
        }
    }
}