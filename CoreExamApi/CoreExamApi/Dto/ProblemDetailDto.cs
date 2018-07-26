using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 争分夺秒答题详情
    /// </summary>
    public class ProblemDetailDto
    {
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid ProblemID { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public string ProblemName { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 准确率
        /// </summary>
        public string Accuracy { get; set; }
    }
}
