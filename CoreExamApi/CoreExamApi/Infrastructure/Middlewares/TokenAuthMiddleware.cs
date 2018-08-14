using CoreExamApi.Infrastructure.Repositorys;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Middlewares
{
    public class TokenAuthMiddleware : IMiddleware
    {
        private readonly IRedisKeyRepository _redisKeyRepository;

        public TokenAuthMiddleware(IRedisKeyRepository redisKeyRepository)
        {
            _redisKeyRepository = redisKeyRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authHeader = context.Request.Headers["Authorization"];
            if (authHeader != StringValues.Empty)
            {
                string jwtStr = authHeader.ToString().Substring("Bearer ".Length).Trim();
                var uid = new JwtSecurityTokenHandler().ReadJwtToken(jwtStr).Claims
                    .Where(x => x.Type == "uid").FirstOrDefault();
                if (uid != null)
                {
                    var tokenStr = await _redisKeyRepository.GetTokenAsync(uid.Value);
                    if (!jwtStr.Equals(tokenStr))
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "text/string";
                        await context.Response.WriteAsync("token 不一致！");
                    }
                }
            }

            await next(context);
        }

    }
}
