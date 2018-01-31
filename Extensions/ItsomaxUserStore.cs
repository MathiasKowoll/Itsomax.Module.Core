using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Extensions
{
    public class ItsomaxUserStore : UserStore<User, Role, ItsomaxDbContext, long, IdentityUserClaim<long>, UserRole,
		IdentityUserLogin<long>, IdentityUserToken<long>, IdentityRoleClaim<long>>
    {
        public ItsomaxUserStore(ItsomaxDbContext context, IdentityErrorDescriber describer) : base(context,describer)
        {
        }
    }
}