using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSoft.Common.Utitlies
{
    public class Utils
    {
        public static string SHA1(string str)
        {
            byte[] cleanBytes = Encoding.UTF8.GetBytes(str);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
    }
}
