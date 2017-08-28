using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.Core.Models
{
    public class AuditLog : EntityBase
    {
        [MaxLength(200)]
        [Required]
        public string ActionTrigered { get; set; }
        [Required]
        public string LogType { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }
        [MaxLength(15)]
        public string IP { get; set; }
        public string Hostname { get; set; }
    }
}
