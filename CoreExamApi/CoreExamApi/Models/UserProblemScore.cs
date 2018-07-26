using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Models
{
    /// <summary>
    /// 考生每道题的成绩
    /// </summary>
    [Table("UserProblemScore")]
    public class UserProblemScore
    {
        [Display(Name = "主键ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        /// <summary>
        /// 预先导入？
        /// </summary>
        #region 题目
        [Display(Name = "考试题问题ID")]
        public Guid ProblemID { get; set; }

        [Display(Name = "考试题问题")]
        [StringLength(500)]
        public string ProblemName { get; set; }

        [Display(Name = "选择题选项")]
        [StringLength(500)]
        public string ProblemFeatures { get; set; }
        

        [Display(Name = "考试题答案")]
        [StringLength(10)]
        public string Answer { get; set; }

        [Display(Name = "问题的分数")]
        public decimal? ProblemScore { get; set; }

        /// <summary>
        /// 题目类型（1、争分夺秒 2、一比高下 3、狭路相逢）
        /// </summary>
        [Display(Name = "考试题类型")]
        public int ProblemType { get; set; }

        /// <summary>
        /// 争分夺秒中为5题一组，其他不区分
        /// </summary>
        [Display(Name = "组")]
        public int ProblemSubType { get; set; }

        /// <summary>
        /// 每组的题号
        /// </summary>
        [Display(Name = "题号")]
        public int QuestionNumber { get; set; }
        #endregion


        [Display(Name = "考试提交时间")]
        public DateTime? ExaminationDate { get; set; }

        [Display(Name = "提交的答案")]
        [StringLength(10)]
        public string SubmitAnswer { get; set; }

        [Display(Name = "得到分数")]
        public decimal? Score { get; set; }

        
        #region 外键
        public Guid UserID { get; set; }
        public virtual User User { get; set; }
        #endregion
    }
}
