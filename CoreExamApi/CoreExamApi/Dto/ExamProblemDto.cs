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
    }
}
