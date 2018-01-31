
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class Modules : EntityBase
    {
        public string Name { get; set; }

        public bool IsValidModule { get; set; }

        public string ShortName { get; set; }
        public string Path { get; set; }
    }
}