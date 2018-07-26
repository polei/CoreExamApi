/*======================================================================
 *
 *        Copyright (C)  1996-2016  杭州品茗安联网事业部    
 *        All rights reserved
 *
 *        Filename :NLoger.cs
 *        DESCRIPTION :
 *
 *        Created By Administrator at 2016-05-25 17:11:01
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System;

namespace PMSoft.Logging
{
    /// <summary>
    /// NLog封装实现
    /// </summary>
    public class NLoger : LoggerBase
    {
        private NLog.Logger _log;
        /// <summary>
        /// 
        /// </summary>
        public NLoger()
        {
            _log = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                case LogLevel.Fatal:
                    return _log.IsFatalEnabled;
                default:
                    return false;
            }
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public override void Log(LogLevel level, string message, Exception exception)
        {
            if (level == LogLevel.Information)
                _log.Info(message);
            else if (level == LogLevel.Debug)
                _log.Debug(message);
            else if (level == LogLevel.Error)
                _log.Error(exception, message);
            else if (level == LogLevel.Warning)
                _log.Warn(message);
            else if (level == LogLevel.Fatal)
                _log.Fatal(exception, message);
            else if (level == LogLevel.Trace)
                _log.Trace(exception, message);

        }
    }
}