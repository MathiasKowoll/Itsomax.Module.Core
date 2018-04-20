using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Itsomax.Module.Core.Extensions
{

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;

            var userName = context.User.Identity.Name.ToUpper();
            var getSession = context.Session.GetString("SessionId");

            if (string.IsNullOrEmpty(getSession))
            {
                    if (!string.IsNullOrEmpty(context.Request.Path))
                    {
                        var redirectTo = string.Format("/login?ReturnUrl={0}",
                            HttpUtility.UrlEncode(context.Request.Path));
                        filterContext.Result = new RedirectResult(redirectTo);
                        return;
                    }
           }
            base.OnActionExecuting(filterContext);
        }
    }
}
