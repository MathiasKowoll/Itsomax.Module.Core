using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.Core.Models
{
    public class EmailConfigurations : EntityBase
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string SmtpServer { get; set; }
        [MaxLength(200)]
        public string FromEmail { get; set; }
        public bool RequireSsl { get; set; }
        public int Port { get; set; }
        [MaxLength(100)]
        public string User { get; set; }
        public byte[] Password { get; set; }
        [MaxLength(100)]
        public string Domain { get; set; }
        [MaxLength(150)]
        public string ApiKey { get; set; }
        public bool Default { get; set; }
    }
}