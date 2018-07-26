//======================================================================
//
//        Copyright (C)  1996-2016  杭州品茗安联网事业部
//        All rights reserved
//
//        Filename : LogHelper
//        Description : 
//
//        版本：V1.0.0.1
//
//        http://www.pinming.cn/
//
//====================================================================== 
using PMSoft.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSoft.Logging
{
    public static class LogHelper
    {
        private static ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = LoggerFactory.GetLog();
                return _logger;
            }
        }
    }
}
