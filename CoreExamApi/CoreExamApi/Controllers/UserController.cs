using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Services;
using CoreExamApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly IIdentityService _identityService;
        public UserController(ExamContext context, IIdentityService identityService)
        {
            _examContext= context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService;
        }
        
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsersList()
        {
            var userId = _identityService.GetUserIdentity();
            var list = await _examContext.Users.ToListAsync();
            return Ok(list);
        }
    }
}