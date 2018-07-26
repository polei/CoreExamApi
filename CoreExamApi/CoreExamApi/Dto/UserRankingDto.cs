using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 用户排名
    /// </summary>
    public class UserRankingDto
    {
        /// <summary>
        /// 排名
        /// </summary>
        public int RankingNum { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public string Score { get; set; }
    }
}
