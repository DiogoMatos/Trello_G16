using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Etapa2.Controllers.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        public string ViewName { get; set; }

        public AuthorizationAttribute()
        {
            ViewName = "LogError";
        }

        public override void OnAuthorization( AuthorizationContext filterContext )
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException( "filterContext" );
            }
            if (!filterContext.HttpContext.User.IsInRole("user"))
            {
                ViewDataDictionary viewData = new ViewDataDictionary();
                viewData.Add("Message", "You have to make the o login first!");
                filterContext.Result = new ViewResult { ViewName = this.ViewName, ViewData = viewData };
            }
        }

        //TODO Check
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}