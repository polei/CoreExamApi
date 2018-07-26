//======================================================================
//
//        Copyright (C)  1996-2016  杭州品茗安联网事业部    
//        All rights reserved
//
//        Filename : ConsoleLoger
//        DESCRIPTION : 控制台日志
//
//        Created By Administrator at  2016-05-25 17:11:01
//        http://www.pinming.cn/
//
//======================================================================   

using System;

namespace PMSoft.Logging
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    public class ConsoleLoger : LoggerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override bool IsEnabled(LogLevel level)
        {
            return true;
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        { 
            Console.WriteLine("Level:{0} {1}  {2}  ", level.ToString(), message, exception != null ? exception.StackTrace : null);
        }
    }
}