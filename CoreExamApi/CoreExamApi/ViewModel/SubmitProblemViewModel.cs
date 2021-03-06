﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    public class SubmitProblemViewModel
    {
        //[Required(ErrorMessage = "答案不能为空。")]
        public string submitAnswer { get; set; }

        [Required(ErrorMessage = "问题Guid不能为空。")]
        public string examProblemID { get; set; }

        /// <summary>
        /// 是否提交,1、表示已经提交
        /// </summary>
        public int? isSubmitOver { get; set; }
    }
}
