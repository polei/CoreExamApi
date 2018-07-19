using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    public class LoginViewModel
    {
        /// <summary>
        /// 手机号码？
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空。")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码手机号码？
        /// </summary>
        [Required(ErrorMessage = "密码不能为空。")]
        public string Password { get; set; }
    }
}
