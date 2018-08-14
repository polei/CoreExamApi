using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Repositorys;
using CoreExamApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoreExamApi.Controllers
{
    [Route("api/v1/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly ExamingSettings _settings;
        private readonly ILogger<AccountController> _logger;
        private readonly IRedisKeyRepository _redisKeyRepository;
        public AccountController(ExamContext examContext
            , IOptionsSnapshot<ExamingSettings> settings
            , ILogger<AccountController> logger
            , IRedisKeyRepository redisKeyRepository)
        {
            _settings = settings.Value;
            _examContext = examContext;
            _logger = logger;
            _redisKeyRepository = redisKeyRepository;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var json = new mJsonResult();
            if (ModelState.IsValid)
            {
                var user = await _examContext.Users
                .SingleOrDefaultAsync(x => x.UserName == model.userName
                && x.UserName == model.password);
                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim("name", user.TrueName),
                        new Claim("sub","Candidate"),
                        new Claim("uid", user.ID.ToString()),
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                                    claims: claims,
                                    expires: DateTime.Now.AddMinutes(5),
                                    signingCredentials: creds
                                );
                    json.success = true;
                    json.data = new JwtSecurityTokenHandler().WriteToken(token);
                    //存redis数据库
                    await _redisKeyRepository.SetTokenAsync(user.ID.ToString(), json.data);

                }
                else
                {
                    json.msg = "用户名或密码错误！";
                }
            }

            return Ok(json);
        }

        [HttpGet]
        [Route("test/logger")]
        public IActionResult TestLogger()
        {
            _logger.LogInformation("整6666");
            return Ok();
        }

        /// <summary>
        /// 大屏幕用户登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("screen/login")]
        public IActionResult ScreenLogin([FromBody] LoginViewModel model)
        {
            var json = new mJsonResult();
            if (ModelState.IsValid)
            {
                if (model.userName== _settings.ScreenLoginName
                    &&model.password== _settings.ScreenPassword)
                {
                    var claims = new[]
                    {
                        new Claim("name", _settings.ScreenLoginName),
                        new Claim("sub","admin"),
                        new Claim("adminPolicy","adminPolicy")
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                                    claims: claims,
                                    expires: DateTime.Now.AddDays(2),
                                    signingCredentials: creds
                                );
                    json.success = true;
                    json.data = new JwtSecurityTokenHandler().WriteToken(token);
                }
                else
                {
                    json.msg = "用户名或密码错误！";
                }
            }

            return Ok(json);
        }
    }
}