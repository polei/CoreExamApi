using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Repositorys
{
    public interface IRedisKeyRepository
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        Task<string> GetTokenAsync(string uid);

        /// <summary>
        /// 插入用户
        /// </summary>
        /// <returns></returns>
        Task<bool> SetTokenAsync(string uid, string tokenStr);
    }
}
