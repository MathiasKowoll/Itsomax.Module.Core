using Microsoft.AspNetCore.Http;
using Itsomax.Module.Core.Interfaces;

namespace Itsomax.Module.Core.Extensions
{
    public class GetRemoteInformation : IGetRemoteInformation
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public GetRemoteInformation(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetIp()
        {
            return _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public string GetHostname()
        {
            return _contextAccessor.HttpContext.Request.Host.Host;
        }
    }
}
