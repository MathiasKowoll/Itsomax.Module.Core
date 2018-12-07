using System.Collections.Generic;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class UserAppSetting : EntityBase
    {
        public UserAppSetting(long id)
        {
            Id = id;
        }
        public string Key { get; set; }
        
        public IList<UserSettingDetail> UserSettingDetail { get; set; } = new List<UserSettingDetail>();
        
    }
}