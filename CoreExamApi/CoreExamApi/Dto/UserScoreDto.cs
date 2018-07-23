using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 用户考试结果
    /// </summary>
    public class UserScoreDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public decimal? TotalScores { get; set; }

        /// <summary>
        /// 争分夺秒分数
        /// </summary>
        public decimal? TypeScores1 { get; set; }

        /// <summary>
        /// 一比高下分数
        /// </summary>
        public decimal? TypeScores2 { get; set; }


        /// <summary>
        /// 狭路相逢分数
        /// </summary>
        public decimal? TypeScores3 { get; set; }
    }
}
