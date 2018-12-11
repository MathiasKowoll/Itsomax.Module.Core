using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
	public class ModuleContentViewModel
	{
		[MaxLength(100)]
		public string Controller { get; set; }
		[MaxLength(100)]
		public long ModulesId { get; set; }


    }
}