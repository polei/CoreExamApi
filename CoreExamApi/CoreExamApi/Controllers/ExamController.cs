using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Dto;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Services;
using CoreExamApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
    /// <summary>
    /// 考试问题
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly IIdentityService _identityService;
        public ExamController(ExamContext context, IIdentityService identityService)
        {
            _examContext = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService;

        }

        /// <summary>
        /// 获取考试问题
        /// </summary>
        /// <param name="problemType"> 题目类型（1、争分夺秒 2、一比高下 3、狭路相逢）</param>
        /// <param name="subType">争分夺秒中为5题一组，其他不区分</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<ExamProblemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProblemList(int problemType = 1, int subType = 0)
        {
            var userId = _identityService.GetUserIdentity();
            var userExamProblemList = await _examContext.UserProblemScores
                .Where(x => x.UserID == new Guid(userId) && x.ProblemType == problemType && x.ProblemSubType == subType)
                .Select(a => new ExamProblemDto
                {
                    ID = a.ID,
                    SubmitAnswer = a.SubmitAnswer,
                    ProblemName = a.ProblemName,
                    ProblemFeatures = a.ProblemFeatures,
                    QuestionNumber = a.QuestionNumber
                }).ToListAsync();
            return Ok(userExamProblemList);
        }

        /// <summary>
        /// 提交一道题目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(mJsonResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveOneProblem(SubmitProblemViewModel model)
        {
            var json =new mJsonResult();
            if (ModelState.IsValid)
            {
                //var userId = _identityService.GetUserIdentity();
                Guid examProblemID = new Guid(model.examProblemID);
                var userProScore = await _examContext.UserProblemScores
                    .FindAsync(examProblemID);
                if (userProScore != null)
                {
                    if (userProScore.SubmitAnswer != model.submitAnswer)
                    {
                        userProScore.ExaminationDate = DateTime.Now;
                        userProScore.SubmitAnswer = model.submitAnswer;
                        string answer = userProScore.Answer
                                .Replace("\n", "").Replace(" ", "")
                                .Replace("\t", "").Replace("\r", "");
                        userProScore.Score = model.submitAnswer == answer
                        ? userProScore.ProblemScore : 0;
                        await _examContext.SaveChangesAsync();
                        json.success = true;
                    }
                }

            }
            return Ok(json);
        }
    }
}