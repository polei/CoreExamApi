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

        [Display(Name = "用户名")]
        [StringLength(200)]
        [Required(ErrorMessage = "手机号码不能为空！")]
        //[RegularExpression(@"^1[34578][0-9]{9}$", ErrorMessage = "手机号格式不正确")]
        public string UserName { get; set; }

        [Display(Name = "姓名")]
        [StringLength(50)]
        [Required(ErrorMessage = "姓名不能为空！")]
        public string TrueName { get; set; }

        [Display(Name = "密码")]
        [StringLength(50)]
        [Required(ErrorMessage = "密码不能为空！")]
        public string Password { get; set; }

        [Display(Name = "性别")]
        public int Sex { get; set; }

        [Display(Name = "出生年")]
        public int? BirthYear { get; set; }

        [Display(Name = "出生月")]
        public int? BirthMonth { get; set; }

        [Display(Name = "单位名称")]
        [StringLength(200)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 默认0未浙江省，1为其他
        /// </summary>
        [Display(Name = "省")]
        public int Province { get; set; }

        [Display(Name = "城市")]
        public int City { get; set; }

        [Display(Name = "地址")]
        [StringLength(100)]
        public string Address { get; set; }

        [Display(Name = "是否为造价工程师")]
        public int IsEngineer { get; set; }

        [Display(Name = "执业注册证书号")]
        [StringLength(200)]
        public string CertificateNumber { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 1、土建 2、安装
        /// </summary>
        [Display(Name = "专业")]
        public int Professional { get; set; }

        /// <summary>
        /// 是否有分公司（0 无，1有）
        /// </summary>
        [Display(Name = "是否有分公司")]
        public int IsChildCompany { get; set; }

        [Display(Name = "分公司所属城市")]
        public int? ChildCity { get; set; }

    }
}
