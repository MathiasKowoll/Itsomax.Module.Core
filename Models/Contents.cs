using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
	public class Contents : EntityBase
    {
        [MaxLength(100)]
        public string Slug { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public long EntityTypeId { get; set; }
        public ContentEntityRoute ContentEntityRoute { get; set; }
    }
}