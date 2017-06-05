using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Extensions
{
    public class ItsomaxUserStore : UserStore<User, Role, ItsomaxDbContext, long, IdentityUserClaim<long>, UserRole,
		IdentityUserLogin<long>, IdentityUserToken<long>, IdentityRoleClaim<long>>
    {
        public ItsomaxUserStore(ItsomaxDbContext context) : base(context)
        {
        }

        protected override UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override IdentityUserClaim<long> CreateUserClaim(User user, Claim claim)
        {
            var userClaim = new IdentityUserClaim<long> { UserId = user.Id };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override IdentityUserLogin<long> CreateUserLogin(User user, UserLoginInfo login)
        {
            return new IdentityUserLogin<long>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override IdentityUserToken<long> CreateUserToken(User user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<long>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }
}