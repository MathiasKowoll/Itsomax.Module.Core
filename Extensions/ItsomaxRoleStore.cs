using System.Security.Claims;
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

        protected override IdentityRoleClaim<long> CreateRoleClaim(Role role, Claim claim)
        {
            return new IdentityRoleClaim<long> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
        }
    }
}