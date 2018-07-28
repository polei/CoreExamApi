using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public AccountController(ExamContext examContext, IOptionsSnapshot<ExamingSettings> settings)
        {
            _settings = settings.Value;
            _examContext = examContext;
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
                        new Claim("uid", user.ID.ToString()),
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