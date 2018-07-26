using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Models
{
    /// <summary>
    /// 当前考试流程
    /// </summary>
    [Table("ExamProcess")]
    public class ExamProcess
    {
        [Display(Name = "主键ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        /// <summary>
        /// 当前流程
        /// </summary>
        public int ModuleType { get; set; }

        /// <summary>
        /// 小组
        /// </summary>
        public int? SubType { get; set; }

        /// <summary>
        /// 当前题数
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
