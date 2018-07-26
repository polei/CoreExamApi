using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.Models
{
    public class BaseSetting
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 一比高下每组时间
        /// </summary>
        public int TypeTimeSpan1 { get; set; }

        /// <summary>
        /// 争分夺秒每题时间
        /// </summary>
        public int TypeTimeSpan2 { get; set; }

        /// <summary>
        /// 狭路相逢每题时间
        /// </summary>
        public int TypeTimeSpan3 { get; set; }

        /// <summary>
        /// 参与时间
        /// </summary>
        public int PartTimeSpan { get; set; }
    }
}