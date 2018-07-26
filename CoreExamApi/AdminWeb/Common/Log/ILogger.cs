//======================================================================
//
//        Copyright (C)  1996-2016  ����Ʒ����������ҵ��    
//        All rights reserved
//
//        Filename : ILogger
//        DESCRIPTION : ��־�ӿ�
//
//        Created By Administrator at  2016-05-25 17:11:01
//        http://www.pinming.cn/
//
//======================================================================  

using System;

namespace PMSoft.Logging 
{
    /// <summary>
    /// ��־�����ӿ�
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel level);
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Log(LogLevel level, Exception exception, string format, params object[] args);
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Log(LogLevel level, string message, Exception exception);
        /// <summary>
        /// ������ʾ��Ϣ��¼
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Log(string message, Exception exception);
        /// <summary>
        /// ��־��¼
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LogLevel level, string message );

        /// <summary>
        /// ��ʾ��Ϣ��¼
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}