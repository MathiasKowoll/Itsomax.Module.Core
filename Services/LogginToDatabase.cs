using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.Models;
using Microsoft.Extensions.Logging;

namespace Itsomax.Module.Core.Services
{
    public class LogginToDatabase : ILogginToDatabase
    {
        private readonly ILogger _logger;
        private readonly IGetRemoteInformation _remote;
        private readonly ItsomaxDbContext _context;

        public LogginToDatabase (ILogger<LogginToDatabase> logger, IGetRemoteInformation remote, 
            ItsomaxDbContext context)
        {
            _logger = logger;
            _remote = remote;
            _context = context;
        }

        public void InformationLog(string message,string action,string detail,string user)
        {
            _logger.LogInformation(message, detail);
            var audit = new AuditLogs
            {
                Message = message,
                Detail = detail,
                Hostname = _remote.GetHostname(),
                Ip = _remote.GetIp(),
                LogType = "Information",
                UserName = user,
                ActionTrigered = action
            };
            _context.Set<AuditLogs>().Add(audit);
            _context.SaveChanges();
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
            var error = new AuditLogs
            {
                Message = exMessage,
                Detail = exceptionDetail,
                Hostname = _remote.GetHostname(),
                Ip = _remote.GetIp(),
                LogType = "Error",
                UserName = user,
                ActionTrigered = action
            };
            _context.Set<AuditLogs>().Add(error);
            _context.SaveChanges();
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
