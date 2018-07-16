using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ExamContext _examContext;
        public UserController(ExamContext context)
        {
            _examContext= context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsersList()
        {
            var list = await _examContext.Users.ToListAsync();
            return Ok(list);
        }
    }
}