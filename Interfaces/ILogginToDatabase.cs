using System;
using System.Collections.Generic;
using System.Text;

namespace Itsomax.Module.Core.Interfaces
{
    public interface ILogginToDatabase
    {
        void InformationLog(string message, string detail, string user);
        void ErrorLog(string exMessage, string exceptionDetail, string user);
    }
}
