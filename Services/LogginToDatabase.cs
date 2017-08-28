using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;
using Microsoft.Extensions.Logging;

namespace Itsomax.Module.Core.Extensions
{
    public class LogginToDatabase : ILogginToDatabase
    {
        private readonly ILogger _logger;
        private readonly IGetRemoteInformation _remote;
        public readonly IRepository<AuditLog> _repoAudit;

        public LogginToDatabase (ILogger<LogginToDatabase> logger, IGetRemoteInformation remote, IRepository<AuditLog> _repoAudit)
        {
            _logger = logger;
            _remote = remote;
        }

        public void InformationLog(string message,string detail,string user)
        {
            _logger.LogInformation(message, detail);
            var audit = new AuditLog { Message = message, Detail = detail, Hostname = _remote.GetHostname(), IP = _remote.GetIp(), LogType = "Information", UserName = user };
            _repoAudit.Add(audit);
            _repoAudit.SaveChange();
        }
        public void ErrorLog(string exMessage,string exceptionDetail,string user)
        {
            _logger.LogError(exMessage, exceptionDetail);
            var audit = new AuditLog { Message = exMessage,Detail = exceptionDetail, Hostname = _remote.GetHostname(), IP = _remote.GetIp(), LogType = "Error", UserName = user };
            _repoAudit.Add(audit);
            _repoAudit.SaveChange();
        }
    }
}
