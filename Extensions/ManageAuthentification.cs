using System.Threading.Tasks;
using Itsomax.Module.Core.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Itsomax.Module.Core.Extensions
{
	public class ManageAuthentification : AuthorizationHandler<ManageAuthentificationRequirement>,IAuthorizationRequirement
	{
        private readonly ItsomaxDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ManageAuthentification(UserManager<User> userManager,ItsomaxDbContext contextDb,IHttpContextAccessor contextAccessor)
		{
            _context = contextDb;
            _contextAccessor = contextAccessor;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAuthentificationRequirement requirement)
		{

           // var path2 = RequestPath.Instance.CurrentContext.Request.Path.Value;
            var userName = context.User.Identity.Name;//ToString();
            //var user = _userManager.FindByNameAsync(userName).Result;
            //var userRoles = _userManager.GetRolesAsync(user);

            var user = _context.Users.FirstOrDefault(x => x.UserName == userName);
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
				//var action = path.Values["action"].ToString();
                if(context.User.HasClaim(x => x.Type==controller && x.Value=="HasAccess"))
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
