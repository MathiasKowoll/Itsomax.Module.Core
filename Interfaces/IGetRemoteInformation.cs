using System;
using System.Collections.Generic;
using System.Text;

namespace Itsomax.Module.Core.Interfaces
{
    public interface IGetRemoteInformation
    {
        string GetIp();
        string GetHostname();
    }
}
