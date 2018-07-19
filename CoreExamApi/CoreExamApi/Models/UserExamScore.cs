using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Models
{
    [Table("UserExamScore")]
    public class UserExamScore
    {
        [Display(Name = "主键ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Display(Name = "争分夺秒分数")]
        public decimal? TypeScores1 { get; set; }

        [Display(Name = "一比高下分数")]
        public decimal? TypeScores2 { get; set; }

        [Display(Name = "狭路相逢分数")]
        public decimal? TypeScores3 { get; set; }

        [Display(Name = "总分")]
        public decimal? TotalScores { get; set; }

        [Display(Name = "是否结束")]
        public int IsOver { get; set; }
        #region 外键
        public Guid UserID { get; set; }
        public virtual User User { get; set; }
        #endregion
    }
}
