using AdminWeb.Services;
using PMSoft.Common.Models;
using PMSoft.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace PMSoft.Security
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userInfoService;
        private readonly TimeSpan _expirationTimeSpan;

        private UserInfo _cachedCustomer;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userInfoService">Customer service</param> 
        public FormsAuthenticationService(HttpContextBase httpContext, IUserService userInfoService)
        {
            this._httpContext = httpContext;
            this._userInfoService = userInfoService;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
        }

        private const string TICKET_USERDATA_FORMAT = "{0}";

        public virtual void SignIn(UserInfo userInfo, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var userData = string.Format(TICKET_USERDATA_FORMAT, userInfo.UserName);
            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                userInfo.UserName,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie, userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            //userInfo = _userInfoService.GetUserInfoByUserName(userInfo.UserName);
            SetUserInfoFromSession(userInfo);
            _httpContext.Response.Cookies.Add(cookie);
            _cachedCustomer = userInfo;
        }

        public virtual void SignOut()
        {
            _cachedCustomer = null;

            FormsAuthentication.SignOut();
            ClearSession();
        }

        public virtual UserInfo GetAuthenticatedUserInfo()
        {
            if (_cachedCustomer != null)
                return _cachedCustomer;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }
            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            if (customer != null)
                _cachedCustomer = customer;
            return _cachedCustomer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual UserInfo GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var username = ticket.UserData;

            if (String.IsNullOrWhiteSpace(username))
                return null;
            var userInfo = GetUserInfoFromSession(username);
            if (userInfo == null)
            {
                userInfo =_userInfoService.GetUserInfoByUserName(username);
                SetUserInfoFromSession(userInfo);
            }
            return userInfo;
        }
        

        /// <summary>
        /// 获取用户Session
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        protected virtual UserInfo GetUserInfoFromSession(string userName)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null) return null;
            var userInfo = HttpContext.Current.Session["PMSoft_FullUserInfo"] as UserInfo;
            if (userInfo == null) return null;
            if (string.Equals(userInfo.UserName, userName, StringComparison.OrdinalIgnoreCase)
                )
                return userInfo;
            HttpContext.Current.Session.Remove("PMSoft_FullUserInfo");
            return null;
        }

        /// <summary>
        /// 设置用户Session
        /// </summary>
        /// <param name="userInfo"></param>
        protected virtual void SetUserInfoFromSession(UserInfo userInfo)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null) return;
            HttpContext.Current.Session["PMSoft_FullUserInfo"] = userInfo;
        }


        private void ClearSession()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null) return;
            HttpContext.Current.Session.Abandon();
        }
        
    }
}
