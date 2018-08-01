﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Dto;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
    /// <summary>
    /// 大屏幕api
    /// </summary>
    //[Authorize]
    [Route("api/v1/screen")]
    [ApiController]
    public class ScreenController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly IIdentityService _identityService;
        private readonly IExamService _examService;
        /// <summary>
        /// 大屏幕
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identityService"></param>
        public ScreenController(ExamContext context
            , IIdentityService identityService, IExamService examService)
        {
            _examContext = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService;
            _examService = examService;

        }
        /// <summary>
        /// 争分夺秒一组问题
        /// </summary>
        /// <param name="subType">组（int）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subProblems")]
        [ProducesResponseType(typeof(ProblemSubDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProblemBySubType(int subType = 1)
        {
            ProblemSubDto model = new ProblemSubDto();
            model.Problems = await _examContext.Problems
                .Where(x => x.ProblemType == (int)eProblemType.争分夺秒
                && x.ProblemSubType == subType).OrderBy(o=>o.QuestionNumber)
                .Select(s => new ProName{ ProblemName = s.ProblemName }).ToListAsync();
            var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == (int)eProblemType.争分夺秒 && x.SubType == subType)
                .FirstOrDefaultAsync();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.Seconds > 0 && baseSetting.TypeTimeSpan1 > tSpan.Seconds)
                {
                    model.Countdown = baseSetting.TypeTimeSpan1 - tSpan.Seconds;
                }
            }
            return Ok(model);
        }

        /// <summary>
        /// 争分夺秒答题详情
        /// </summary>
        /// <param name="subType">组（int）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("subProblemsDetail")]
        [ProducesResponseType(typeof(List<ProblemDetailDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubProblemDetail(int subType = 1)
        {
            List<ProblemDetailDto> list = new List<ProblemDetailDto>();
            var details = await _examContext.Problems
                .Where(x => x.ProblemType == (int)eProblemType.争分夺秒
                && x.ProblemSubType == subType)
                .OrderBy(o => o.QuestionNumber).ToListAsync();
            foreach (var detail in details)
            {
                var model = new ProblemDetailDto();
                model.ProblemID = detail.ID;
                model.ProblemName = detail.ProblemName;
                model.Answer = detail.Answer;
                model.SubmitCount = await _examContext.UserProblemScores
                    .Where(x => x.ProblemID == detail.ID && x.SubmitAnswer != null).CountAsync();
                model.RightCount = await _examContext.UserProblemScores
                    .Where(x => x.ProblemID == detail.ID && x.SubmitAnswer == detail.Answer).CountAsync();
                model.Accuracy = model.RightCount + "/" + model.SubmitCount;
                list.Add(model);
            }
            return Ok(list);
        }

        /// <summary>
        /// 获取单题答题详情
        /// (包含争分夺秒，一比高下，狭路相逢)
        /// </summary>
        /// <param name="problemID">问题的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("singleProblemDetail")]
        [ProducesResponseType(typeof(SingleProblemDetailDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSingleProblemDetail(Guid problemID)
        {
            SingleProblemDetailDto model = new SingleProblemDetailDto();
            var problem = await _examContext.Problems.FindAsync(problemID);
            if (problem != null)
            {
                model.ProblemName = problem.ProblemName;
                model.ProblemFeatures = problem.ProblemFeatures;
                model.Answer = problem.Answer;
                model.NumberArr = await _examContext.UserProblemScores
                    .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer)
                    .Select(s => s.User.OrderNumber).ToArrayAsync();
                if (problem.ProblemType != (int)eProblemType.争分夺秒)
                {
                    model.SubmitCount = await _examContext.UserProblemScores
                    .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer != null).CountAsync();
                    model.RightCount = await _examContext.UserProblemScores
                        .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer).CountAsync();
                    model.Accuracy= model.RightCount + "/" + model.SubmitCount;
                }
            }
            return Ok(model);
        }


        /// <summary>
        /// 一道题目（一比高下和狭路相逢）
        /// </summary>
        /// <param name="problemType">问题类型（2、一比高下 3、狭路相逢）</param>
        /// <param name="questionNumber">问题编号（int）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problemDetail")]
        [ProducesResponseType(typeof(ProblemSingleWithCountDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProblemDetail(int problemType = 2, int questionNumber = 1)
        {
            ProblemSingleWithCountDto model = new ProblemSingleWithCountDto();
            model.Problem = await _examContext.Problems
                .Where(x => x.ProblemType == problemType
                && x.QuestionNumber == questionNumber)
                .Select(s => new ProblemSingle
                {
                    ProblemID=s.ID,
                    ProblemName =s.ProblemName,
                    ProblemFeatures=s.ProblemFeatures
                }).SingleOrDefaultAsync();
            if(problemType== (int)eProblemType.一比高下)
            {
                if (questionNumber == 1)
                {
                    model.PartUserCount = await _examContext.UserProblemScores
                        .Where(x => x.ProblemType == problemType&& x.QuestionNumber == questionNumber)
                        .CountAsync();
                }
                else
                {
                    model.PartUserCount = await _examContext.UserProblemScores
                        .Where(x => x.ProblemType == problemType 
                        && x.QuestionNumber == questionNumber-1&&x.Score>0)
                        .CountAsync();
                }
                
            }
            if(problemType == (int)eProblemType.狭路相逢)
            {
                model.PartUserCount = await _examContext.UserExamPartners
                    .Where(x => x.QuestionNumber == questionNumber).CountAsync();
            }
            var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == problemType && x.Number == questionNumber)
                .FirstOrDefaultAsync();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                var tTimeSpan = problemType == (int)eProblemType.一比高下
                    ? baseSetting.TypeTimeSpan2 : baseSetting.TypeTimeSpan3;
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.Seconds > 0 && tTimeSpan > tSpan.Seconds)
                {
                    model.Countdown = tTimeSpan - tSpan.Seconds;
                }
            }
            return Ok(model);
        }

        /// <summary>
        /// 获取一道题目答对的人数
        /// </summary>
        /// <param name="problemID">问题的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problem/right/count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRightProblemCount(Guid problemID)
        {
            var rightCount = await _examContext.UserProblemScores
                .Where(x => x.ProblemID == problemID&&x.Score>0).CountAsync();
            return Ok(rightCount);
        }


        /// <summary>
        /// 单个问题晋级人数统计
        /// </summary>
        /// <param name="problemID">问题统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("passUserCount")]
        [ProducesResponseType(typeof(UserProblemCountDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserProblemCount(Guid problemID)
        {
            UserProblemCountDto model = new UserProblemCountDto();
            var problem = await _examContext.Problems.FindAsync(problemID);
            if (problem != null)
            {
                model.ProblemName = problem.ProblemName;
                model.Answer = problem.Answer;
                model.AllNumberArr = await _examContext.Users
                    .Where(x=>x.UserName!="admin").OrderBy(o => o.OrderNumber)
                    .Select(s => s.OrderNumber).ToArrayAsync();
                model.NumberArr = await _examContext.UserProblemScores.Include(c=>c.User)
                    .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer)
                    .OrderBy(o=>o.User.OrderNumber)
                    .Select(s => s.User.OrderNumber).ToArrayAsync();
                if (problem.ProblemType != (int)eProblemType.争分夺秒)
                {
                    model.PassUserCount = await _examContext.UserProblemScores
                        .Where(x => x.ProblemID == problem.ID 
                        && x.SubmitAnswer == problem.Answer).CountAsync();
                }
            }
            return Ok(model);
        }

        /// <summary>
        /// 用户排名
        /// </summary>
        /// <param name="problemType">类型（1、争分夺秒 2、一比高下 3、狭路相逢）不传获取总分排名</param>
        /// <returns></returns>
        [HttpGet]
        [Route("userRanking")]
        [ProducesResponseType(typeof(List<UserRankingDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserRankingList(int? problemType)
        {
            var userRanking = await _examService.GetUserRankingList(problemType);
            return Ok(userRanking);
        }
    }
}