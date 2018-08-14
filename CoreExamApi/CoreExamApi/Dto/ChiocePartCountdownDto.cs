using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    public class ChiocePartCountdownDto
    {
        /// <summary>
        /// 用户选择（1是）
        /// </summary>
        public int ChiocePart { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public int Countdown { get; set; }
    }
}
