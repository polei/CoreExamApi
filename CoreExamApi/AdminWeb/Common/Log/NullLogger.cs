//======================================================================
//
//        Copyright (C)  1996-2016  杭州品茗安联网事业部    
//        All rights reserved
//
//        Filename : NullLogger
//        DESCRIPTION : 日志类型
//
//        Created By Administrator at  2016-05-25 17:11:01
//        http://www.pinming.cn/
//
//======================================================================   

using System;

namespace PMSoft.Logging
{ 
    /// <summary>
    /// 空日志
    /// </summary>
    public class NullLogger : LoggerBase
    {
        /// <summary>
        /// 空日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        {

        }
    }
}
