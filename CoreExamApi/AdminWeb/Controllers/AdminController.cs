using AdminWeb.Extensions;
using PMSoft.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminWeb.Controllers
{
    [AdminAuthorizeAttribute]
    public class AdminController : Controller
    {
        private readonly IWorkContext _workContext;

        public AdminController()
        {
            _workContext = DependencyResolver.Current.GetService<IWorkContext>();
        }
        
        /// <summary>
        /// 获取当前工作上下文
        /// </summary> 
        /// <returns></returns>
        public IWorkContext WorkContext
        {
            get { return _workContext; }
        }

        /// <summary>
        /// 获取当前登录用户
        /// </summary> 
        /// <returns></returns>
        public UserInfo UserInfo
        {
            get
            {
                if (WorkContext != null) return WorkContext.CurrentUser;
                return null;
            }
            set
            {
                if (WorkContext != null)
                    WorkContext.CurrentUser = value;
            }
        }

        /// <summary>
        ///  登录用户ID
        /// </summary>
        public Guid UserId
        {
            get { return UserInfo != null ? UserInfo.UserId : Guid.Empty; }
        }
        
    }
}