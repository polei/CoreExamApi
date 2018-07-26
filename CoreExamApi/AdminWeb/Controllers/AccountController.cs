using AdminWeb.Extensions;
using AdminWeb.Services;
using AdminWeb.ViewModel;
using PMSoft.Common.Models;
using PMSoft.Logging;
using PMSoft.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AdminWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        public AccountController(ILogger logger
            , IAuthenticationService authenticationService
            ,  IWorkContext workContext
            , IUserService userService)
        {
            _userService = userService;
            _logger = logger;
            _workContext = workContext;
            _authenticationService = authenticationService;
        }
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginInfoViewModel loginInfo)
        {
            var json = new mJsonResult();
            try
            {
                if (loginInfo.UserName != "admin")
                {
                    json.msg = "无法登录!";
                    return Json(json);
                }
                var user = await _userService.GetUserFromLogin(loginInfo.UserName, loginInfo.UserPassword);
                if (user!=null)
                {
                    _authenticationService.SignIn(new UserInfo
                    {
                        UserId = user.ID,
                        UserName = user.UserName
                    }, true);
                    json.success = true;
                    json.msg = "登录成功";
                    return Json(json);
                }
                else
                {
                    json.msg = "用户名或密码错误！";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("登录异常" + ex.Message);
                json.msg = "登录异常！";
            }
            return Json(json);
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (_workContext.CurrentUser != null 
                && _workContext.CurrentUser.UserId != Guid.Empty
                && User.Identity.IsAuthenticated)
            {
                _authenticationService.SignOut();
            }
            return RedirectToAction("Login");
        }
    }
}