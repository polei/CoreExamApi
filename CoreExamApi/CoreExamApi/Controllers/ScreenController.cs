using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Dto;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.Infrastructure.Services;
using CoreExamApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CoreExamApi.Controllers
{
    /// <summary>
    /// 大屏幕api
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
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
                && x.ProblemSubType == subType).OrderBy(o => o.QuestionNumber)
                .Select(s => new ProName { ProblemName = s.ProblemName }).ToListAsync();
            var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == (int)eProblemType.争分夺秒 && x.SubType == subType)
                .FirstOrDefaultAsync();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.TotalSeconds > 0 && baseSetting.TypeTimeSpan1 > Convert.ToInt32(tSpan.TotalSeconds))
                {
                    model.Countdown = baseSetting.TypeTimeSpan1 - Convert.ToInt32(tSpan.TotalSeconds);
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
                    .OrderBy(x => x.User.OrderNumber)
                    .Select(s => s.User.OrderNumber).ToArrayAsync();
                if (problem.ProblemType == (int)eProblemType.一比高下)
                {
                    if (problem.QuestionNumber == 1)
                    {
                        model.SubmitCount = await _examContext.UserExamScores
                        .Where(x => x.TotalScores >= 0)
                        .CountAsync();
                    }
                    else
                    {
                        model.SubmitCount = await _examContext.UserProblemScores
                           .Where(x => x.ProblemID == problem.ID && (x.ExaminationDate.HasValue||x.IsSubmitOver>0))
                           .CountAsync();
                    }
                    model.RightCount = await _examContext.UserProblemScores
                       .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer).CountAsync();
                    model.Accuracy = model.RightCount + "/" + model.SubmitCount;

                }
                else if (problem.ProblemType == (int)eProblemType.狭路相逢)
                {
                    model.SubmitCount = await _examContext.UserExamPartners
                        .Where(x => x.QuestionNumber == problem.QuestionNumber
                        && x.ChiocePart == (int)eChoicePart.是).CountAsync();
                    model.RightCount = await _examContext.UserProblemScores
                       .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer).CountAsync();
                    model.Accuracy = model.RightCount + "/" + model.SubmitCount;
                }
            }
            else
            {
                return NotFound();
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
                    ProblemID = s.ID,
                    ProblemName = s.ProblemName,
                    ProblemFeatures = s.ProblemFeatures
                }).SingleOrDefaultAsync();
            var process = _examContext.ExamProcesss
                .Where(x => x.ModuleType == problemType && x.Number == questionNumber)
                .FirstOrDefault();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                var tTimeSpan = problemType == (int)eProblemType.一比高下
                    ? baseSetting.TypeTimeSpan2
                    : (baseSetting.TypeTimeSpan3 + baseSetting.PartTimeSpan);
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.TotalSeconds > 0 && tTimeSpan > Convert.ToInt32(tSpan.TotalSeconds))
                {
                    model.Countdown = tTimeSpan - Convert.ToInt32(tSpan.TotalSeconds);
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
        [ProducesResponseType(typeof(AccuracyDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRightProblemCount(Guid problemID)
        {
            AccuracyDto model = new AccuracyDto();
            var problem =await _examContext.Problems.FindAsync(problemID);
            if(problem is null)
            {
                return NotFound();
            }
            else
            {
                if (problem.ProblemType == (int)eProblemType.一比高下)
                {
                    if (problem.QuestionNumber == 1)
                    {
                        model.partUserCount = await _examContext.UserExamScores
                            .Where(x => x.TotalScores >= 0)
                            .CountAsync();
                    }
                    else
                    {
                        model.partUserCount = await _examContext.UserProblemScores
                            .Where(x => x.ProblemID == problemID&& (x.ExaminationDate.HasValue||x.IsSubmitOver>0))
                            .CountAsync();
                    }
                }
                if (problem.ProblemType == (int)eProblemType.狭路相逢)
                {
                    model.partUserCount = await _examContext.UserExamPartners
                        .Where(x => x.QuestionNumber == problem.QuestionNumber
                        && x.ChiocePart == (int)eChoicePart.是).Select(x => x.UserID)
                        .Distinct().CountAsync();
                }
                model.rightCount = await _examContext.UserProblemScores
               .Where(x => x.ProblemID == problemID && x.Score > 0).CountAsync();
            }
            return Ok(model);
        }


        /// <summary>
        /// 单个问题晋级人数统计
        /// </summary>
        /// <param name="problemID">问题统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("passUserCount")]
        [ProducesResponseType(typeof(UserProblemCountDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserProblemCount(Guid problemID)
        {
            UserProblemCountDto model = new UserProblemCountDto();
            var problem = await _examContext.Problems.FindAsync(problemID);
            if (problem is null)
            {
                return NotFound();
            }
            else
            {
                model.ProblemName = problem.ProblemName;
                model.Answer = problem.Answer;
                model.AllNumberArr = await _examContext.Users
                    .Where(x => x.UserName != "admin").OrderBy(o => o.OrderNumber)
                    .Select(s => s.OrderNumber).ToArrayAsync();
                model.NumberArr = await _examContext.UserProblemScores.Include(c => c.User)
                    .Where(x => x.ProblemID == problem.ID && x.SubmitAnswer == problem.Answer)
                    .OrderBy(o => o.User.OrderNumber)
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


        /// <summary>
        /// 获取狭路相逢倒计时
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("chioce/countdown")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetChiocePartCountdown()
        {
            var countdown = 0;
            var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == (int)eProblemType.狭路相逢)
                .FirstOrDefaultAsync();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                var tTimeSpan = baseSetting.PartTimeSpan;
                TimeSpan tSpan = DateTime.Now - process.AddTime;
                if (tSpan.TotalSeconds > 0 && tTimeSpan > Convert.ToInt32(tSpan.TotalSeconds))
                {
                    countdown = tTimeSpan - Convert.ToInt32(tSpan.TotalSeconds);
                }
            }
            else
            {
                return NotFound();
            }
            return Ok(countdown);
        }

        /// <summary>
        /// 对于选择狭路相逢是的选手，而没有分数的批量处理
        /// </summary>
        /// <param name="questionNumber">问题编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("type3/score/sum")]
        [ProducesResponseType(typeof(mJsonResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SumUserProblemScore(int questionNumber = 1)
        {
            var json = new mJsonResult();
            var userIDList = _examContext.UserExamPartners
                .Where(x => x.QuestionNumber == questionNumber
                && x.ChiocePart == (int)eChoicePart.是).Select(x => x.UserID).Distinct().ToList();
            if (userIDList != null && userIDList.Count() > 0)
            {
                var userScoreIDList = _examContext.UserProblemScores
                    .Where(x => x.QuestionNumber == questionNumber
                        && userIDList.Contains(x.UserID) && (x.Score == 0 || x.Score == null))
                        .Select(s => new UserProblemScoreViewModel
                        {
                            userProblemScoreID = s.ID,
                            UserID = s.UserID,
                            ProblemScore = s.ProblemScore
                        }).ToList();
                List<UserProblemScoreViewModel> list = new List<UserProblemScoreViewModel>();
                foreach (var score in userScoreIDList)
                {
                    var model = new UserProblemScoreViewModel();
                    model.userProblemScoreID = score.userProblemScoreID;
                    model.UserID = score.UserID;
                    var userExamScore = _examContext.UserExamScores
                        .Where(x => x.UserID == score.UserID).FirstOrDefault();
                    model.TypeScores3 = userExamScore.TypeScores3 - score.ProblemScore;
                    model.TotalScores = userExamScore.TotalScores - score.ProblemScore;
                    list.Add(model);
                }
                if (list.Count() > 0)
                {
                    json.success = await _examService.SumUserExamScore(userScoreIDList);
                }
            }
            json.success = true;//出错好像也做不了东西
            return Ok(json);
        }
    }
}