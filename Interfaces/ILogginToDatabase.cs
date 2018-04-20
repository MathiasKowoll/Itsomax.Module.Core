using Itsomax.Module.Core.ViewModels;

namespace Itsomax.Module.Core.Interfaces
{
    public interface ILogginToDatabase
    {
        void InformationLog(string message, string action, string detail, string user);
        void InformationLog(string message, string action, string detail);
        void InformationLog(string message, string action);
        void ErrorLog(string exMessage, string action, string exceptionDetail, string user);
        void ErrorLog(string message, string action, string exceptionDetail);
        void ErrorLog(string message, string action);
        SuccessErrorHandling SuccessErrorHandlingTask(string loggerMessage, string successErrorType, string toasterMessage,
            bool succeeded);
    }
}
