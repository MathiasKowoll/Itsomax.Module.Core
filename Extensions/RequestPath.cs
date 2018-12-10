using Microsoft.AspNetCore.Http;

namespace Itsomax.Module.Core.Extensions
{
	public class RequestPath
	{
		public static RequestPath Instance { get; set; }

		static RequestPath()
		{
			Instance = new RequestPath(null);
		}
		private readonly IHttpContextAccessor _contextAccessor;

		public RequestPath(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		public HttpContext CurrentContext
		{
			get
			{
			if (_contextAccessor == null)
				return null;
			return _contextAccessor.HttpContext;
			}
		}
	}
}
