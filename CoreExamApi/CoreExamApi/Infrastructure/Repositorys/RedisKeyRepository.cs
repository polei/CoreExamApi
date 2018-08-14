using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Repositorys
{
    /// <summary>
    /// 存储用户ID和token
    /// </summary>
    public class RedisKeyRepository: IRedisKeyRepository
    {
        private readonly ILogger<RedisKeyRepository> _logger;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        public RedisKeyRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisKeyRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync(string uid)
        {
            var data = await _database.StringGetAsync(uid);
            return data;
        }

        /// <summary>
        /// 插入用户
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetTokenAsync(string uid,string tokenStr)
        {
            var created = await _database.StringSetAsync(uid, tokenStr);
            if (!created)
            {
                _logger.LogInformation("插入用户出错????");
            }
            return created;
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}
