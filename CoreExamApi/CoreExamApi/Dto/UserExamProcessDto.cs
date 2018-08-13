using CoreExamApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 用户当前流程进度（主要是判断在狭路相逢的时候）
    /// </summary>
    public class UserExamProcessDto:ExamProcess
    {
        /// <summary>
        /// 是否参与（0、否；1、是）
        /// </summary>
        public int? ChiocePart { get; set; }

        /// <summary>
        /// 是否选择结束
        /// </summary>
        public bool IsChioceOver { get; set; }
        ///// <summary>
        ///// 当前流程是否结束（0、否；1是）
        ///// </summary>
        //public int IsOver { get; set; }

        /// <summary>
        /// 未结束倒计时
        /// </summary>
        public int Countdown
        { get; set; }
    }
}
