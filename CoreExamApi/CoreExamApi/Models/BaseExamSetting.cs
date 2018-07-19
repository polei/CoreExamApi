using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Models
{
    /// <summary>
    /// 基本设置（？）
    /// </summary>
    [Table("BaseExamSetting")]
    public class BaseExamSetting
    {
        [Display(Name = "主键ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        /// <summary>
        /// 一比高下每组时间
        /// </summary>
        public int TypeTimeSpan1 { get;set;}

        /// <summary>
        /// 争分夺秒每题时间
        /// </summary>
        public int TypeTimeSpan2 { get; set; }

        /// <summary>
        /// 狭路相逢每题时间
        /// </summary>
        public int TypeTimeSpan3 { get; set; }

    }
}
