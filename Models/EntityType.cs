using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class EntityType : EntityBase
    {
        public string Name { get; set; }

        public bool IsMenuable { get; set; }

        public string RoutingController { get; set; }

        public string RoutingAction { get; set; }
    }
}