
using PMSoft.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSoft.Security
{
    /// <summary>
    /// 授权服务
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="createPersistentCookie"></param>
        void SignIn(UserInfo userInfo, bool createPersistentCookie);
        /// <summary>
        /// 退出
        /// </summary>
        void SignOut();

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <returns></returns>
        UserInfo GetAuthenticatedUserInfo();
        
    }
}
