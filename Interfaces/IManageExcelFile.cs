using System;
using System.Collections.Generic;

namespace Itsomax.Module.Core.Interfaces
{
    public interface IManageExcelFile
    {
        //string CreateExcelFile(string exelName);
        IList<string> GenerateExcelName(string reportName, DateTime reportDate);
    }
}
