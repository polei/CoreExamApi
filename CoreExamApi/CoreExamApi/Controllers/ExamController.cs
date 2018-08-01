﻿using System;
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
        private readonly IExamService _examService;
        public ExamController(ExamContext context
            , IIdentityService identityService
            , IExamService examService)
        {
            _examContext = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService;
            _examService = examService;

        }

        /// <summary>
        /// 获取考试问题1、争分夺秒
        /// </summary>
        /// <param name="subType">争分夺秒中为5题一组，其他不区分</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problems")]
        [ProducesResponseType(typeof(ExamProblemWithTimeDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProblemList(int subType = 0)
        {
            var userId = _identityService.GetUserIdentity();
            var userExamProblemList = await _examContext.UserProblemScores
                .Where(x => x.UserID == new Guid(userId) 
                && x.ProblemType == (int)eProblemType.争分夺秒 
                && x.ProblemSubType == subType)
                .OrderBy(o => o.QuestionNumber)
                .Select(a => new ExamProblemDto
                {
                    ID = a.ID,
                    SubmitAnswer = a.SubmitAnswer,
                    ProblemName = a.ProblemName,
                    ProblemFeatures = a.ProblemFeatures,
                    QuestionNumber = a.QuestionNumber
                }).ToListAsync();
            ExamProblemWithTimeDto model = new ExamProblemWithTimeDto();
            model.ProblemList = userExamProblemList;
            var process = await _examContext.ExamProcesss
                .Where(x=>x.ModuleType== (int)eProblemType.争分夺秒 && x.SubType == subType)
                .FirstOrDefaultAsync();
            if (process!=null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.Seconds > 0&& baseSetting.TypeTimeSpan1 > tSpan.Seconds)
                {
                    model.Countdown = baseSetting.TypeTimeSpan1 - tSpan.Seconds;
                }
            }
            
            return Ok(model);
        }

        /// <summary>
        /// 获取考试问题（2、一比高下 3、狭路相逢）
        /// </summary>
        /// <param name="problemType">题目类型(2、一比高下 3、狭路相逢)</param>
        /// <param name="questionNumber">问题编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problem")]
        [ProducesResponseType(typeof(ExamProblemDto2), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProblem(int problemType = 2,int questionNumber=1)
        {
            var userId = _identityService.GetUserIdentity();
            var userExamProblem = await _examContext.UserProblemScores
                .Where(x => x.UserID == new Guid(userId)
                && x.ProblemType == problemType
                && x.QuestionNumber == questionNumber).SingleOrDefaultAsync();
            ExamProblemDto2 model = new ExamProblemDto2();
            if (userExamProblem!=null)
            {
                model.ID = userExamProblem.ID;
                model.ProblemName = userExamProblem.ProblemName;
                model.ProblemFeatures = userExamProblem.ProblemFeatures;
                model.SubmitAnswer = userExamProblem.SubmitAnswer;
                model.QuestionNumber = userExamProblem.QuestionNumber;
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
            }
           
            return Ok(model);
        }

        /// <summary>
        /// 获取每道题目的结果（一比高下？）
        /// </summary>
        /// <param name="userExamProblemID">用户考题的ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("problem/result")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProblemResult(Guid userExamProblemID)
        {
            bool result = false;
            var userExamProblem = await _examContext.UserProblemScores.FindAsync(userExamProblemID);
            if (userExamProblem != null)
            {
                result = userExamProblem.Score > 0;
            }
            return Ok(result);
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
            var json = new mJsonResult();
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
                        if(userProScore.ProblemType== (int)eProblemType.狭路相逢)
                        {
                            userProScore.Score = model.submitAnswer == answer
                                ? userProScore.ProblemScore : -userProScore.ProblemScore;
                        }
                        else
                        {
                            userProScore.Score = model.submitAnswer == answer
                                ? userProScore.ProblemScore : 0;
                        }
                        var userId = _identityService.GetUserIdentity();
                        var userExamScore = await _examContext.UserExamScores.SingleOrDefaultAsync(s => s.UserID == new Guid(userId));
                        var userProblemScore = _examContext.UserProblemScores.Where(x => x.Score.HasValue
                                      && x.UserID == new Guid(userId));
                        switch (userProScore.ProblemType)
                        {
                            case (int)eProblemType.争分夺秒:
                                userExamScore.TypeScores1 = await userProblemScore.Where(x => x.ProblemType == (int)eProblemType.争分夺秒)
                                    .SumAsync(s => s.Score) + userProScore.Score;
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
                        userExamScore.TotalScores = (userExamScore.TypeScores1??0)
                            + (userExamScore.TypeScores2??0) + (userExamScore.TypeScores3??0);
                        _examContext.UserExamScores.Update(userExamScore);
                        await _examContext.SaveChangesAsync();
                        json.success = true;
                    }
                }

            }
            return Ok(json);
        }

        /// <summary>
        /// 狭路相逢选择是否参与
        /// </summary>
        /// <param name="questionNumber">问题编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("partner")]
        [ProducesResponseType(typeof(mJsonResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveUserExamPartner(int questionNumber=1)
        {
            var json = new mJsonResult();
            var userId = _identityService.GetUserIdentity();
            UserExamPartner model = new UserExamPartner
            {
                QuestionNumber = questionNumber,
                UserID = new Guid(userId),
                AddTime = DateTime.Now
            };
            _examContext.UserExamPartners.Add(model);
            json.success= await _examContext.SaveChangesAsync()>0;
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
        public async Task<IActionResult> GetModuleDetail(int type = 1)
        {
            ModuleDetailDto model = new ModuleDetailDto();
            var userId = _identityService.GetUserIdentity();
            var userProScore = await _examContext.UserProblemScores
                   .Where(x => x.UserID == new Guid(userId)).ToListAsync();
            var userTypeProScore = userProScore.Where(x=>x.ProblemType==type);
            model.tAnswerCount = userTypeProScore.Count(c => c.Score > 0);
            model.wAnswerCount = userTypeProScore.Count(c => c.Score < 1);
            model.mScore = userTypeProScore.Sum(s => s.Score);
            model.allScore = userProScore.Sum(s => s.Score);
            switch (type)
            {
                case (int)eProblemType.狭路相逢:
                    model.ranking = await _examContext.UserExamScores
                        .Where(o => o.TotalScores > model.allScore).CountAsync() + 1;
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
            var userScore = await _examContext.UserExamScores.Include(c=>c.User)//默认不开启懒加载
                .SingleOrDefaultAsync(x => x.UserID == new Guid(userId));
            if (userScore != null)
            {
                model.TrueName = userScore.User?.TrueName;
                model.TotalScores = userScore.TotalScores;
                model.TypeScores1 = userScore.TypeScores1;
                model.TypeScores2 = userScore.TypeScores2;
                model.TypeScores3 = userScore.TypeScores3;
                model.ranking = await _examContext.UserExamScores
                            .Where(o => o.TotalScores > model.TotalScores).CountAsync() + 1;
            }
            return Ok(model);
        }

        /// <summary>
        /// 获取排名（手机顶部）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("userRanking")]
        [ProducesResponseType(typeof(SingleUserRankingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserRanking()
        {
            SingleUserRankingDto model = new SingleUserRankingDto();
            var userId = _identityService.GetUserIdentity();
            var userRankingList = await _examService.GetUserRankingList();
            var userRanking = userRankingList.Where(x => x.UserID == new Guid(userId)).Single();
            model.xRanking = userRanking.RankingNum;
            model.xScore = userRanking.Score;
            var userLessRanking = userRankingList.Where(x => x.Score > model.xScore)
                .OrderByDescending(o => o.Score).FirstOrDefault();
            if (userLessRanking != null)
            {
                model.xLessRanking = userLessRanking.RankingNum;
                model.xLessScore = userLessRanking.Score;
            }
            var userPlusRanking = userRankingList.Where(x => x.Score < model.xScore)
                .OrderByDescending(o => o.Score).FirstOrDefault();
            if (userPlusRanking!=null)
            {
                model.xPlusRanking = userPlusRanking.RankingNum;
                model.xPlusScore = userPlusRanking.Score;
            }
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

        /// <summary>
        /// 获取当前流程进度
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("process")]
        [ProducesResponseType(typeof(ExamProcess), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExamProcess()
        {
            var process = await _examContext.ExamProcesss.FirstOrDefaultAsync();
            return Ok(process);
        }
    }
}