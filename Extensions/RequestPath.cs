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
		private readonly IHttpContextAccessor contextAccessor;

		public RequestPath(IHttpContextAccessor contextAccessor)
		{
			this.contextAccessor = contextAccessor;
		}

		public HttpContext CurrentContext
		{
			get
			{
			if (contextAccessor == null)
				return null;
			return contextAccessor.HttpContext;
			}
		}
	}
}
