using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi
{
    public class ExamingSettings
    {
        /// <summary>
        /// 秘钥
        /// </summary>
        public string SecurityKey { get; set; }
        
        /// <summary>
        /// 大屏幕用户名
        /// </summary>
        public string ScreenLoginName { get; set; }

        /// <summary>
        /// 大屏幕密码
        /// </summary>
        public string ScreenPassword { get; set; }
    }
}
