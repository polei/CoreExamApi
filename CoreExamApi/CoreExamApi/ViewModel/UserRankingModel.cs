using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRankingModel
    {
        /// <summary>
        /// 排名
        /// </summary>
        public int RankingNum { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>

        public Guid UserID { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal? Score { get; set; }
    }
}
