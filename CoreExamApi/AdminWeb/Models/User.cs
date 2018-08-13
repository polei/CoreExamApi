using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.Models
{
    public class User
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 登录为手机号码，密码也为手机号码
        /// </summary>
        public string UserName { get; set; }
        
        public string TrueName { get; set; }

        
        public string CompanyName { get; set; }
        
        public int OrderNumber { get; set; }
        
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否造价工程师(1、是 0、否)
        /// </summary>
        public int IsEngineer { get; set; }
    }
}