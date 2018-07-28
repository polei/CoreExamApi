using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 单个问题晋级人数统计
    /// </summary>
    public class UserProblemCountDto
    {
        /// <summary>
        /// 问题
        /// </summary>
        public string ProblemName { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 答对选手的编号
        /// </summary>
        public int[] NumberArr { get; set; }

        /// <summary>
        /// 所有选手的编号
        /// </summary>
        public int[] AllNumberArr { get; set; }

        /// <summary>
        /// 通过人数（晋级人数）
        /// </summary>
        public int PassUserCount { get; set; }
    }
}
