using System.Collections.Generic;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class UserAppSetting : EntityBase
    {
        public string SettingType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public IList<UserSettingDetail> UserSettingDetail { get; set; } = new List<UserSettingDetail>();
        
    }
}