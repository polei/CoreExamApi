using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.Models
{
    public class Problem
    {
        public Guid ID { get; set; }
        
        public string ProblemName { get; set; }
        
        public string ProblemFeatures { get; set; }
        
        public string Answer { get; set; }
        
        public decimal? Score { get; set; }
        /// <summary>
        /// 题目类型（1、争分夺秒 2、一比高下 3、狭路相逢）
        /// </summary>
        public int ProblemType { get; set; }

        /// <summary>
        /// 争分夺秒中为5题一组，其他不区分
        /// </summary>
        public int ProblemSubType { get; set; }

        /// <summary>
        /// 每组的题号
        /// </summary>
        public int QuestionNumber { get; set; }
    }
}