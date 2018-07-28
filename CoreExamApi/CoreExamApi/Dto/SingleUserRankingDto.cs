using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 当前用户排名
    /// </summary>
    public class SingleUserRankingDto
    {
        /// <summary>
        /// 当前排名
        /// </summary>
        public int xRanking { get; set; }

        /// <summary>
        /// 当前分数
        /// </summary>
        public decimal? xScore { get; set; }

        /// <summary>
        /// 比当前高的排名
        /// </summary>
        public int xPlusRanking { get; set; }

        /// <summary>
        /// 比当前高的分数
        /// </summary>
        public decimal? xPlusScore { get; set; }

        /// <summary>
        /// 比当前低的排名
        /// </summary>
        public int xLessRanking { get; set; }
        /// <summary>
        /// 比当前低的分数
        /// </summary>
        public decimal? xLessScore { get; set; }
    }
}
