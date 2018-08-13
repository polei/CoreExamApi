using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Dto
{
    public class ExamProblemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }
        //[Display(Name = "考试题问题")]
        public string ProblemName { get; set; }

        //[Display(Name = "选择题选项")]
        public string ProblemFeatures { get; set; }


        //[Display(Name = "考试题答案")]
        public string SubmitAnswer { get; set; }
        
        /// <summary>
        /// 每组的题号
        /// </summary>
        public int QuestionNumber { get; set; }

        /// <summary>
        /// 是否已提交（1、代表已提交）
        /// </summary>
        public int IsSubmitOver { get; set; }
    }

    /// <summary>
    /// 争分夺秒每一组的问题
    /// </summary>
    public class ExamProblemWithTimeDto
    {
        /// <summary>
        /// 每一组里面的问题
        /// </summary>
        public List<ExamProblemDto> ProblemList { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public int Countdown { get; set; }
        /// <summary>
        /// 是否已提交（1、代表已提交）
        /// </summary>
        public int? IsSubmitOver { get; set; }
    }


    /// <summary>
    /// 一比高下和狭路相逢问题
    /// </summary>
    public class ExamProblemDto2 : ExamProblemDto
    {
        /// <summary>
        /// 倒计时
        /// </summary>
        public int Countdown { get; set; }
    }
}
