using System.Threading.Tasks;
using Itsomax.Module.Core.Events;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Itsomax.Module.Core.Extensions
{
	public class ItsomaxSignInManager<TUser> : SignInManager<TUser> where TUser : class
	{
		private readonly IMediator _mediator;

        public ItsomaxSignInManager(UserManager<TUser> userManager,
			IHttpContextAccessor contextAccessor,
			IUserClaimsPrincipalFactory<TUser> claimsFactory,
			IOptions<IdentityOptions> optionsAccessor,
			ILogger<SignInManager<TUser>> logger,
			IAuthenticationSchemeProvider schemes,
			IMediator mediator)
			: base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
		{
			_mediator = mediator;
		}

		public override async Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
		{
			var userId = await UserManager.GetUserIdAsync(user);
			await _mediator.Publish(new UserSignedIn { UserId = long.Parse(userId) });
			await base.SignInAsync(user, isPersistent, authenticationMethod);
		}
	}
}