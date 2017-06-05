using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Itsomax.Module.Core.Extensions
{
	public class ManageAuthentification : AuthorizationHandler<ManageAuthentification>,IAuthorizationRequirement
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAuthentification requirement)
		{

            //var path2 = RequestPath.Instance.CurrentContext.Request.Path.Value;
			var user = context.User.ToString();
			if (context.User.Identity.IsAuthenticated)
			{
				if (context.User.IsInRole("Admin"))
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				}

                var path = RequestPath.Instance.CurrentContext.Request.Path.Value;
                //string path;
                if (context.User.HasClaim(c => c.Type.ToString() == "HasAccess" && path.ToString().Contains(c.Value.ToString())))
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				}
				else
				{
					context.Fail();
					return Task.CompletedTask;
				}
			}
			else
			{
                context.Fail();
				return Task.CompletedTask;
			}
		}
	}
}
