using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    public class SingleProblemDetailDto
    {
        /// <summary>
        /// 问题
        /// </summary>
        public string ProblemName { get; set; }

        /// <summary>
        /// 题目选项
        /// </summary>
        public string ProblemFeatures { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 答对选手的编号
        /// </summary>
        public int[] NumberArr { get; set; }

        /// <summary>
        /// 准确率
        /// </summary>
        public string Accuracy { get; set; }
    }
}
