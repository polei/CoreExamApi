using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PMSoft.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0)
            {
                return members[0].ToDescription();
            }
            return enumeration.CastTo<string>();
        }
    }
}
