using PMSoft.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdminWeb.Extensions
{
    public interface IWorkContext
    {
        /// <summary>
        /// 登录用户
        /// </summary>
        UserInfo CurrentUser { get; set; }

        HttpCookie GetTokenIDCookie();

        string GetSessionId();
        void SetTokenIDCookie(string tokenID);

        /// <summary>
        /// 登录用户ID
        /// </summary>
        Guid CurrentUserId { get; }

        void RefeshSession();
    }
}
