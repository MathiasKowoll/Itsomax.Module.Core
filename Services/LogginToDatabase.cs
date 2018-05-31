using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;
using Microsoft.Extensions.Logging;

namespace Itsomax.Module.Core.Services
{
    public class LogginToDatabase : ILogginToDatabase
    {
        private readonly ILogger _logger;
        private readonly IGetRemoteInformation _remote;
        private readonly IRepository<AuditLogs> _repoAudit;

        public LogginToDatabase (ILogger<LogginToDatabase> logger, IGetRemoteInformation remote, IRepository<AuditLogs> repoAudit)
        {
            _logger = logger;
            _remote = remote;
            _repoAudit = repoAudit;
        }

        public void InformationLog(string message,string action,string detail,string user)
        {
            _logger.LogInformation(message, detail);
            var audit = new AuditLogs { Message = message, Detail = detail, Hostname = _remote.GetHostname(), Ip = _remote.GetIp(), LogType = "Information", UserName = user,ActionTrigered=action };
            _repoAudit.Add(audit);
            _repoAudit.SaveChanges();
        }

        public void InformationLog(string message, string action, string detail)
        {
            InformationLog(message,action, detail, string.Empty);
        }

        public void InformationLog(string message, string action)
        {
            InformationLog(message, string.Empty, string.Empty);
        }


        public void ErrorLog(string exMessage, string action, string exceptionDetail,string user)
        {
            _logger.LogInformation(exMessage, exceptionDetail);
            var error = new AuditLogs { Message = exMessage,Detail = exceptionDetail, Hostname = _remote.GetHostname(), Ip = _remote.GetIp(), LogType = "Error", UserName = user,ActionTrigered = action };
            _repoAudit.Add(error);
            _repoAudit.SaveChanges();
        }

        public void ErrorLog(string message, string action, string exceptionDetail)
        {
            ErrorLog(message, action, exceptionDetail, string.Empty);
        }

        public void ErrorLog(string message, string action)
        {
            ErrorLog(message,action, string.Empty, string.Empty);
        }
    }
}
