using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class Role : IdentityRole<long, UserRole, IdentityRoleClaim<long>>, IEntityWithTypedId<long>
    {
    }
}