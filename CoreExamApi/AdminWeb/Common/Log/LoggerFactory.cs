//======================================================================
//
//        Copyright (C)  1996-2016  杭州品茗安联网事业部    
//        All rights reserved
//
//        Filename : LoggerFactory
//        DESCRIPTION : 日志工厂
//
//        Created By Administrator at  2016-05-25 17:11:01
//        http://www.pinming.cn/
//
//======================================================================   

using System;
using System.Configuration;
using PMSoft.Utitlies;

namespace PMSoft.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public class LoggerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerType"></param>
        /// <returns></returns>
        public static ILogger GetLog(LoggerType loggerType)
        {
            switch (loggerType)
            {
                case LoggerType.ConsoleLog:
                    return DefaultSingleton<ConsoleLoger>.GetInstance();
                case LoggerType.NLog:
                    return DefaultSingleton<NLoger>.GetInstance();
                default:
                    return DefaultSingleton<NLoger>.GetInstance();
            }
        }

        /// <summary>
        /// 根据AppSettings["LoggerType"]配置获取日志类型
        /// </summary>
        /// <returns></returns>
        public static ILogger GetLog()
        {
            return GetLog(CurrentLoggerType);
        }

        /// <summary>
        /// 获取当前AppSettings["LoggerType"]配置的日志类型
        /// </summary>
        public static LoggerType CurrentLoggerType
        {
            get
            {
                string logTypeSettings = ConfigurationManager.AppSettings["LoggerType"];
                if (string.IsNullOrEmpty(logTypeSettings)) return LoggerType.NullLog;
                bool isDefined = TypeParse.IsNumber(logTypeSettings) ?
                    Enum.IsDefined(typeof(LoggerType), TypeParse.StrToInt(logTypeSettings)) :
                    Enum.IsDefined(typeof(LoggerType), logTypeSettings);
                return isDefined ? (LoggerType)Enum.Parse(typeof(LoggerType), logTypeSettings)
                              : LoggerType.NullLog;
            }
        }
    }


}