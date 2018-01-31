using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Itsomax.Module.Core.Extensions
{
	public class ManageAuthentification : AuthorizationHandler<ManageAuthentificationRequirement>,IAuthorizationRequirement
	{
        private readonly IHttpContextAccessor _contextAccessor;

        public ManageAuthentification(IHttpContextAccessor contextAccessor)
		{
            _contextAccessor = contextAccessor;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAuthentificationRequirement requirement)
		{

           // var path2 = RequestPath.Instance.CurrentContext.Request.Path.Value;
            //var userName = context.User.Identity.Name;//ToString();
            //var user = _userManager.FindByNameAsync(userName).Result;
            //var userRoles = _userManager.GetRolesAsync(user);

            //var user = _context.Users.FirstOrDefault(x => x.UserName == userName);
            //var userRoles = _context.UserRoles.Where(x => x.UserId == user.Id).AsQueryable();
            //var modRoles = _context.ModuleRole

            //var path = RequestPath.Instance.CurrentContext.Request.Path.Value;

            

            if (context.User.Identity.IsAuthenticated)
			{
				if (context.User.IsInRole("Admin"))
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				}
				var path = _contextAccessor.HttpContext.GetRouteData();
				var controller = path.Values["controller"].ToString();
				var action = path.Values["action"].ToString();

                if(action=="WelcomePage")
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                if (action == "ChangePasswordView")
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if (context.User.HasClaim(x => x.Type==controller && x.Value=="HasAccess"))
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
