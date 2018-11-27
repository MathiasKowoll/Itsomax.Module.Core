using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class User : IdentityUser<long>, IEntityWithTypedId<long>
    {
        public User()
        {
            //CreatedOn = DateTimeOffset.Now;
            UpdatedOn = DateTimeOffset.Now;
        }
        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; }
        public IList<UserRole> Roles { get; set; } =  new List<UserRole>();
        public IList<UserSetting> UserSetting { get; set; } = new List<UserSetting>(); 
    }
}