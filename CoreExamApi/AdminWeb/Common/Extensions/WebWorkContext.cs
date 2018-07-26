using PMSoft.Common.Models;
using PMSoft.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.Extensions
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public class WebWorkContext: IWorkContext
    {
        #region Const

        private const string UserInfoCookieName = "PMSoft.UserInfo";
        private const string TokenIDCookieName = "PMSoft.TokenID";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IAuthenticationService _authenticationService;

        private UserInfo _cachedUserinfo;

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            IAuthenticationService authenticationService
            )
        {
            this._httpContext = httpContext;
            this._authenticationService = authenticationService;
        }

        #endregion

        #region Utilities

        public virtual string GetSessionId()
        {
            //if (_httpContext == null || _httpContext.Request == null)
            //    return null;

            //return _httpContext.Request.Cookies["ASP.NET_SessionId"];

            if (HttpContext.Current == null || HttpContext.Current.Session == null) return null;
            return HttpContext.Current.Session.SessionID;
        }

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserInfoCookieName];
        }

        protected virtual void SetCustomerCookie(Guid userId)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserInfoCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userId.ToString();
                if (userId == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24; //TODO cookie 过期时间设置
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(UserInfoCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }


        /// <summary>
        /// 获取tokenID的值
        /// </summary>
        /// <returns></returns>
        public virtual HttpCookie GetTokenIDCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[TokenIDCookieName];
        }
        /// <summary>
        /// 把tokenID输出到客户端（判断唯一性）
        /// </summary>
        /// <param name="tokenID"></param>
        public virtual void SetTokenIDCookie(string tokenID)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(TokenIDCookieName);
                cookie.HttpOnly = true;
                cookie.Value = tokenID;
                if (tokenID == (Guid.Empty).ToString())
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24; //TODO cookie 过期时间设置
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(TokenIDCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual UserInfo CurrentUser
        {
            get
            {
                if (_cachedUserinfo != null)
                    return _cachedUserinfo;
                var userInfo = _authenticationService.GetAuthenticatedUserInfo();
                if (userInfo != null)
                {
                    SetCustomerCookie(userInfo.UserId);
                    //TokenID
                    //SetTokenIDCookie(_authenticationService.GetUserTokenID(userInfo.UserName));
                    _cachedUserinfo = userInfo;
                }
                return _cachedUserinfo;
            }
            set
            {
                SetCustomerCookie(value.UserId);
                _cachedUserinfo = value;
            }
        }

        /// <summary>
        /// 登录用户ID
        /// </summary>
        public Guid CurrentUserId
        {
            get { return CurrentUser != null ? CurrentUser.UserId : Guid.Empty; }
        }

        public void RefeshSession()
        {
            if (HttpContext.Current.Session["PMSoft_FullUserInfo"] == null)
            {
                _authenticationService.GetAuthenticatedUserInfo();
            }
        }

        #endregion
    }
}