using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    /// <summary>
    /// 参与的viewModel
    /// </summary>
    public class UserPartnerViewModel
    {
        /// <summary>
        /// 问题编号
        /// </summary>
        public int questionNumber { get; set; }

        /// <summary>
        /// 是否参与,默认不参与（1、参与，0、不参与）
        /// </summary>
        public int chiocePart { get; set; }
    }
}
