using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Dto;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.Infrastructure.Services;
using CoreExamApi.Models;
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
    [Route("api/v1/exam")]
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
        [Route("problems")]
        [ProducesResponseType(typeof(List<ExamProblemDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProblemList(int problemType = 1, int subType = 0)
        {
            var userId = _identityService.GetUserIdentity();
            var userExamProblemList = await _examContext.UserProblemScores
                .Where(x => x.UserID == new Guid(userId) && x.ProblemType == problemType && x.ProblemSubType == subType)
                .OrderBy(o=>o.QuestionNumber)
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
        [Route("saveOneProblem")]
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

                        var userId= _identityService.GetUserIdentity();
                        var userExamScore =await _examContext.UserExamScores.SingleOrDefaultAsync(s=>s.UserID== new Guid(userId));
                        var userProblemScore = _examContext.UserProblemScores.Where(x => x.Score.HasValue
                                      && x.UserID == new Guid(userId));
                        switch (userProScore.ProblemType)
                        {
                            case (int)eProblemType.争分夺秒:
                                userExamScore.TypeScores1=await userProblemScore.Where(x=>x.ProblemType== (int)eProblemType.争分夺秒)
                                    .SumAsync(s=>s.Score)+ userProScore.Score;
                                break;
                            case (int)eProblemType.一比高下:
                                userExamScore.TypeScores2 = await userProblemScore.Where(x => x.ProblemType == (int)eProblemType.一比高下)
                                    .SumAsync(s => s.Score) + userProScore.Score;
                                break;
                            case (int)eProblemType.狭路相逢:
                                userExamScore.TypeScores3 = await userProblemScore.Where(x => x.ProblemType == (int)eProblemType.狭路相逢)
                                    .SumAsync(s => s.Score) + userProScore.Score;
                                break;
                        }
                        userExamScore.TotalScores = userExamScore.TypeScores1
                            + userExamScore.TypeScores2 + userExamScore.TypeScores3;
                        _examContext.UserExamScores.Update(userExamScore);
                        await _examContext.SaveChangesAsync();
                        json.success = true;
                    }
                }

            }
            return Ok(json);
        }

        /// <summary>
        /// 获取每个模块结束的分数信息
        /// </summary>
        /// <param name="type">模块e</param>
        /// <returns></returns>
        [HttpGet]
        [Route("detail")]
        [ProducesResponseType(typeof(ModuleDetailDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetModuleDetail(int type=1)
        {
            ModuleDetailDto model = new ModuleDetailDto();
            var userId = _identityService.GetUserIdentity();
            var userProScore = await _examContext.UserProblemScores
                   .Where(x=>x.UserID== new Guid(userId)).ToListAsync();
            model.tAnswerCount = userProScore.Count(c=>c.Score>0);
            model.wAnswerCount = userProScore.Count(c=>c.Score<1);
            model.mScore = userProScore.Sum(s => s.Score);
            model.allScore = userProScore.Where(x=>x.ProblemType== type).Sum(s => s.ProblemScore);
            switch (type)
            {
                case (int)eProblemType.狭路相逢:
                    model.ranking = 0; 
                        //_examContext.UserExamScores.OrderByDescending(o=>o.TotalScores).
                    break;
            }
            return Ok(model);
        }

        /// <summary>
        /// 获取用户的考试结果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("userScores")]
        [ProducesResponseType(typeof(UserScoreDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserScores()
        {
            UserScoreDto model = new UserScoreDto();
            var userId = _identityService.GetUserIdentity();
            var userScore = await _examContext.UserExamScores
                .SingleOrDefaultAsync(x => x.UserID == new Guid(userId));
            model.TrueName = userScore.User.TrueName;
            model.TotalScores = userScore.TotalScores;
            model.TypeScores1 = userScore.TypeScores1;
            model.TypeScores2 = userScore.TypeScores2;
            model.TypeScores3 = userScore.TypeScores3;
            return Ok(model);
        }

        /// <summary>
        /// 获取时间倒计时区间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getTimeSpans")]
        [ProducesResponseType(typeof(BaseExamSetting), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTimeSpans()
        {
            var examSetting = await _examContext.BaseExamSettings.SingleAsync();

            return Ok(examSetting);
        }
    }
}