﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.Core.Models
{
    public class AuditLogs
    {
        public AuditLogs()
        {
            CreatedOn = DateTimeOffset.Now;
        }

        [MaxLength(200)]
        [Required]
        public string ActionTrigered { get; set; }
        [Required]
        public string LogType { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }
        [MaxLength(50)]
        public string Ip { get; set; }
        public string Hostname { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
