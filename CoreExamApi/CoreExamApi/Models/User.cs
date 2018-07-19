using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Models
{
    [Table("User")]
    public class User
    {
        [Display(Name = "主键ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        /// <summary>
        /// 登录为手机号码，密码也为手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        [StringLength(200)]
        [Required(ErrorMessage = "手机号码不能为空！")]
        public string UserName { get; set; }

        [Display(Name = "姓名")]
        [StringLength(50)]
        [Required(ErrorMessage = "姓名不能为空！")]
        public string TrueName { get; set; }
        

        [Display(Name = "单位名称")]
        [StringLength(200)]
        public string CompanyName { get; set; }

        [Display(Name ="序号")]
        public int OrderNumber { get; set; }
        

        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }
        

    }
}
