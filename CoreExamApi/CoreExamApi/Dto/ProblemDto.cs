using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    /// <summary>
    /// 大屏幕问题plus倒计时
    /// </summary>
    public class ProblemSubDto
    {
        /// <summary>
        /// 一组问题
        /// </summary>
        public List<ProName> Problems { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public int Countdown { get; set; }
    }

    public class ProName
    {
        public string ProblemName { get; set; }
    }

    /// <summary>
    /// 单个问题
    /// </summary>
    public class ProblemSingleWithCountDto
    {
       /// <summary>
       /// 一个问题
       /// </summary>
        public ProblemSingle Problem { get; set; }
        /// <summary>
        /// 倒计时
        /// </summary>
        public int Countdown { get; set; }

        /// <summary>
        /// 参与人数
        /// </summary>
        public int PartUserCount { get; set; }
    }

    public class ProblemSingle
    {
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid ProblemID { get; set; }
        /// <summary>
        /// 问题名称
        /// </summary>
        public string ProblemName { get; set; }

        /// <summary>
        /// 问题选项
        /// </summary>
        public string ProblemFeatures { get; set; }
    }
}
