using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class ContentEntityRoute : EntityBase
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string RoutingController { get; set; }
        [MaxLength(200)]
        public string RoutingAction { get; set; }
        
    }
}