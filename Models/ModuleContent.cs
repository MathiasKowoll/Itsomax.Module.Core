using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;
namespace Itsomax.Module.Core.Models
{
	public class ModuleContent : EntityBase
	{
		[MaxLength(100)]
		public string Controller { get; set; }
		[MaxLength(100)]
		public string Action { get; set; }
		[MaxLength(100)]
		public string ReturnType { get; set; }
		[MaxLength(100)]
		public string Attributes { get; set; }
		public string Path { get; set; }
		public long ModulesId { get; set; }
		public Modules Modules { get; set; }


    }
}