using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 准确率
    /// </summary>
    public class AccuracyDto
    {
        /// <summary>
        /// 正确人数
        /// </summary>
        public int rightCount { get; set; }

        /// <summary>
        /// 提交人数
        /// </summary>
        public int partUserCount { get; set; }
    }
}
