using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class AppSetting : EntityBase
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}