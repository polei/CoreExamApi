using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminWeb.Extensions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AdminAuthorizeAttribute: AuthorizeAttribute
    {
        private IEnumerable<AdminAuthorizeAttribute> GetAdminAuthorizeAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(AdminAuthorizeAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AdminAuthorizeAttribute), true))
                .OfType<AdminAuthorizeAttribute>();
        }

        private bool IsAdminPageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetAdminAuthorizeAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
                return true;
            return false;
        }

        /// <summary>
        /// 是否为匿名访问
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private bool IsAllowAnonymous(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                             .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true))
                             .OfType<AllowAnonymousAttribute>().Any();
        }
        private bool HasAdminAccess(AuthorizationContext filterContext)
        {
            IWorkContext workContext = DependencyResolver.Current.GetService<IWorkContext>();
            return workContext.CurrentUser != null && workContext.CurrentUser.UserId != Guid.Empty;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [AdminAuthorize] attribute when a child action cache is active");
            var flag = IsAllowAnonymous(filterContext.ActionDescriptor);
            if (!flag && IsAdminPageRequested(filterContext))
            {
                if (!this.HasAdminAccess(filterContext))
                    this.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}