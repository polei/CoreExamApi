using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    public class ModuleDetailDto
    {
        /// <summary>
        /// 答对数量
        /// </summary>
        public int tAnswerCount { get; set; }

        /// <summary>
        /// 答错数量
        /// </summary>
        public int wAnswerCount { get; set; }

        /// <summary>
        /// 模块分数
        /// </summary>
        public decimal? mScore { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public decimal? allScore { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public int ranking { get; set; }
    }
}
