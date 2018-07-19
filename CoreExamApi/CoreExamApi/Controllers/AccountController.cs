using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
    [Route("api/v1/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ExamContext _examContext;
        public AccountController(ExamContext examContext)
        {
            _examContext = examContext;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user=await _examContext.Users.SingleOrDefaultAsync(x=>x.UserName==model.UserName);
            return Ok();
        }
    }
}