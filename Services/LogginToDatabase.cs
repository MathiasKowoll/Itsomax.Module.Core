using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;
using Itsomax.Module.Core.ViewModels;
using Microsoft.Extensions.Logging;

namespace Itsomax.Module.Core.Services
{
    public class LogginToDatabase : ILogginToDatabase
    {
        private readonly ILogger _logger;
        private readonly IGetRemoteInformation _remote;
        public readonly IRepository<AuditLogs> RepoAudit;
        public readonly IRepository<ErrorLog> RepoError;

        public LogginToDatabase (ILogger<LogginToDatabase> logger, IGetRemoteInformation remote, IRepository<AuditLogs> repoAudit, IRepository<ErrorLog> repoError)
        {
            _logger = logger;
            _remote = remote;
            RepoAudit = repoAudit;
            RepoError = repoError;
        }

        public SuccessErrorHandling SuccessErrorHandlingTask(string loggerMessage, string successErrorType, string toasterMessage, bool succeeded)
        {
            return new SuccessErrorHandling
            {
                LoggerMessage = loggerMessage,
                SuccessErrorType = successErrorType,
                ToasterMessage = toasterMessage,
                Succeeded = succeeded
            };
        }

        public void InformationLog(string message,string action,string detail,string user)
        {
            _logger.LogInformation(message, detail);
            var audit = new AuditLogs { Message = message, Detail = detail, Hostname = _remote.GetHostname(), Ip = _remote.GetIp(), LogType = "Information", UserName = user,ActionTrigered=action };
            RepoAudit.Add(audit);
            RepoAudit.SaveChanges();
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
            _logger.LogError(exMessage, exceptionDetail);
            var error = new ErrorLog { Message = exMessage,Detail = exceptionDetail, Hostname = _remote.GetHostname(), Ip = _remote.GetIp(), LogType = "Error", UserName = user,ActionTrigered = action };
            RepoError.Add(error);
            RepoError.SaveChanges();
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
