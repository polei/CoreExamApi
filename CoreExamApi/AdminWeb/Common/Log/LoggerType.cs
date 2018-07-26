//======================================================================
//
//        Copyright (C)  1996-2016  杭州品茗安联网事业部    
//        All rights reserved
//
//        Filename : LoggerType
//        DESCRIPTION : 日志类型,日志等级
//
//        Created By Administrator at  2016-05-25 17:11:01
//        http://www.pinming.cn/
//
//======================================================================   
namespace PMSoft.Logging
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// 空日志
        /// </summary>
        NullLog = 0,
        /// <summary>
        /// 控制台日志
        /// </summary>
        ConsoleLog = 1,
        /// <summary>
        /// Log4Net日志
        /// </summary>
        Log4Net = 2,
        /// <summary>
        /// NLog日志
        /// </summary>
        NLog = 3,
    }
}