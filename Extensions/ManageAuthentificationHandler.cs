﻿using System.Threading.Tasks;
using Itsomax.Module.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Itsomax.Module.Core.Extensions
{
	public class ManageAuthentificationHandler : AuthorizationHandler<ManageAuthentificationRequirement>
	{
        private readonly IHttpContextAccessor _contextAccessor;
	    private readonly ItsomaxDbContext _dbContext;

        public ManageAuthentificationHandler(IHttpContextAccessor contextAccessor,ItsomaxDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAuthentificationRequirement requirement)
		{


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
