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
        public readonly IRepository<ErrorLog> _repoError;

        public LogginToDatabase (ILogger<LogginToDatabase> logger, IGetRemoteInformation remote, IRepository<AuditLog> repoAudit, IRepository<ErrorLog> repoError)
        {
            _logger = logger;
            _remote = remote;
            _repoAudit = repoAudit;
            _repoError = repoError;
        }

        public void InformationLog(string message,string action,string detail,string user)
        {
            _logger.LogInformation(message, detail);
            var audit = new AuditLog { Message = message, Detail = detail, Hostname = _remote.GetHostname(), IP = _remote.GetIp(), LogType = "Information", UserName = user,ActionTrigered=action };
            _repoAudit.Add(audit);
            _repoAudit.SaveChange();
        }

        public void InformationLog(string message, string action, string detail)
        {
            InformationLog(message,action, detail, string.Empty);
        }

        public void InformationLog(string message, string Action)
        {
            InformationLog(message, string.Empty, string.Empty);
        }


        public void ErrorLog(string exMessage, string action, string exceptionDetail,string user)
        {
            _logger.LogError(exMessage, exceptionDetail);
            var error = new ErrorLog { Message = exMessage,Detail = exceptionDetail, Hostname = _remote.GetHostname(), IP = _remote.GetIp(), LogType = "Error", UserName = user,ActionTrigered = action };
            _repoError.Add(error);
            _repoError.SaveChange();
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
