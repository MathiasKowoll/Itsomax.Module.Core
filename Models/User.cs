using System;
//using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class User : IdentityUser<long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityUserToken<long>>, IEntityWithTypedId<long>
    {
        public User()
        {
            CreatedOn = DateTimeOffset.Now;
            UpdatedOn = DateTimeOffset.Now;
        }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}