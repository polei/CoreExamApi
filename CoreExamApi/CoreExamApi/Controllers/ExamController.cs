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
using Microsoft.Extensions.Logging;

namespace CoreExamApi.Controllers
{
    /// <summary>
    /// 考试问题
    /// </summary>
    [Authorize(Policy = "CandidateOnly")]
    [Route("api/v1/exam")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly IIdentityService _identityService;
        private readonly IExamService _examService;
        private readonly ILogger<ExamController> _logger;
        public ExamController(ExamContext context
            , IIdentityService identityService
            , IExamService examService
            , ILogger<ExamController> logger)
        {
            _examContext = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService;
            _examService = examService;
            _logger = logger;
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
                    QuestionNumber = a.QuestionNumber,
                    IsSubmitOver=a.IsSubmitOver
                }).ToListAsync();
            ExamProblemWithTimeDto model = new ExamProblemWithTimeDto();
            model.ProblemList = userExamProblemList;
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
        /// 获取考试问题（2、一比高下 3、狭路相逢）
        /// </summary>
        /// <param name="problemType">题目类型(2、一比高下 3、狭路相逢)</param>
        /// <param name="questionNumber">问题编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problem")]
        [ProducesResponseType(typeof(ExamProblemDto2), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetExamProblem(int problemType = 2, int questionNumber = 1)
        {
            //_logger.LogInformation("获取狭路相逢考试问题：problemType：{0}，questionNumber：{1}", problemType, questionNumber);
            var userId = _identityService.GetUserIdentity();
            var userExamProblem = await _examContext.UserProblemScores
                .Where(x => x.UserID == new Guid(userId)
                && x.ProblemType == problemType
                && x.QuestionNumber == questionNumber).SingleOrDefaultAsync();
            ExamProblemDto2 model = new ExamProblemDto2();
            if (userExamProblem != null)
            {
                model.ID = userExamProblem.ID;
                model.ProblemName = userExamProblem.ProblemName;
                model.ProblemFeatures = userExamProblem.ProblemFeatures;
                model.SubmitAnswer = userExamProblem.SubmitAnswer;
                model.IsSubmitOver = userExamProblem.IsSubmitOver;
                model.QuestionNumber = userExamProblem.QuestionNumber;
                var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == problemType && x.Number == questionNumber)
                .FirstOrDefaultAsync();
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
            }
            else
            {
                return NotFound();
            }

            return Ok(model);
        }

        /// <summary>
        /// 获取每道题目的结果（一比高下？）
        /// </summary>
        /// <param name="userExamProblemID">用户考题的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("problem/result")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetExamProblemResult(Guid userExamProblemID)
        {
            bool result = false;
            var userExamProblem = await _examContext.UserProblemScores.FindAsync(userExamProblemID);
            if (userExamProblem != null)
            {
                result = userExamProblem.Score > 0;
            }
            else
            {
                return NotFound();
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SaveOneProblem(SubmitProblemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var json = new mJsonResult();
            Guid examProblemID = new Guid(model.examProblemID);
            using (var trans = _examContext.Database.BeginTransaction())
            {
                try
                {
                    var userProScore = await _examContext.UserProblemScores
                                           .FindAsync(examProblemID);
                    if (userProScore != null)
                    {
                        #region 判断（规定时间内答题）
                        if(!await GetProblemInCountdown(userProScore))
                        {
                            json.msg = "只能在规定时间内答题";
                            return Ok(json);
                        }
                        #endregion

                        if (userProScore.SubmitAnswer != model.submitAnswer)
                        {
                            userProScore.ExaminationDate = DateTime.Now;
                            userProScore.SubmitAnswer = model.submitAnswer;
                            if (model.isSubmitOver.HasValue)
                            {
                                userProScore.IsSubmitOver = model.isSubmitOver.Value;//代表已提交
                            }
                            string answer = userProScore.Answer
                                    .Replace("\n", "").Replace(" ", "")
                                    .Replace("\t", "").Replace("\r", "");
                            if (userProScore.ProblemType == (int)eProblemType.狭路相逢)
                            {
                                userProScore.Score = model.submitAnswer == answer
                                    ? userProScore.ProblemScore : -userProScore.ProblemScore;
                            }
                            else
                            {
                                userProScore.Score = model.submitAnswer == answer
                                    ? userProScore.ProblemScore : 0;
                            }
                            _examContext.UserProblemScores.Update(userProScore);
                            _examContext.SaveChanges();
                            //统计所有
                            var userId = new Guid(_identityService.GetUserIdentity());
                            var userExamScore = await _examContext.UserExamScores
                                .SingleOrDefaultAsync(s => s.UserID == userId);
                            var userProblemScore = _examContext.UserProblemScores
                                .Where(x => x.Score.HasValue && x.UserID == userId);
                            switch (userProScore.ProblemType)
                            {
                                case (int)eProblemType.争分夺秒:
                                    userExamScore.TypeScores1 = userProblemScore
                                        .Where(x => x.ProblemType == (int)eProblemType.争分夺秒)
                                        .Sum(s => s.Score);
                                    break;
                                case (int)eProblemType.一比高下:
                                    userExamScore.TypeScores2 =userProblemScore
                                        .Where(x => x.ProblemType == (int)eProblemType.一比高下)
                                        .Sum(s => s.Score);
                                    break;
                                case (int)eProblemType.狭路相逢:
                                    userExamScore.TypeScores3 = userProblemScore
                                        .Where(x => x.ProblemType == (int)eProblemType.狭路相逢)
                                        .Sum(s => s.Score);
                                    break;
                            }
                            userExamScore.TotalScores = (userExamScore.TypeScores1 ?? 0)
                                + (userExamScore.TypeScores2 ?? 0) + (userExamScore.TypeScores3 ?? 0);

                            _examContext.UserExamScores.Update(userExamScore);
                            _examContext.SaveChanges();

                            trans.Commit();//显式事物提交
                            json.success = true;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }catch(Exception ex)
                {
                    trans.Rollback();
                    _logger.LogError("保存一道题目的时候出错:{0}",ex.Message);
                    //throw new Exception();
                }
            }

            return Ok(json);
        }

        /// <summary>
        /// 争锋夺秒提交每一组的状态
        /// </summary>
        /// <param name="subType">状态</param>
        /// <returns></returns>
        [HttpPost]
        [Route("save/sub/status")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SaveProblemStatusType1(int subType=1)
        {
            var userId = new Guid(_identityService.GetUserIdentity());
            var userProScoreList = _examContext.UserProblemScores
                .Where(x => x.ProblemType == (int)eProblemType.争分夺秒
                && x.UserID == userId && x.ProblemSubType == subType)
                .Select(s =>new UserProScoreIDModel { scoreID=s.ID } ).ToList();
            if(userProScoreList is null)
            {
                return NotFound();
            }
            bool result =await _examService.UpdateUserProStatus(userProScoreList);
            return Ok(result);
        }

        /// <summary>
        /// 一比高下提交每一组的状态
        /// </summary>
        /// <param name="questionNumber">题目编号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("save/number/status")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SaveProblemStatusType2(int questionNumber = 1)
        {
            var userId = new Guid(_identityService.GetUserIdentity());
            var userProScore = _examContext.UserProblemScores
                .Where(x => x.ProblemType == (int)eProblemType.一比高下
                && x.UserID == userId && x.QuestionNumber == questionNumber).FirstOrDefault();
            if (userProScore is null)
            {
                return NotFound();
            }
            userProScore.IsSubmitOver = 1;
            _examContext.UserProblemScores.Update(userProScore);
            bool result = await _examContext.SaveChangesAsync()>0;
            return Ok(result);
        }

        /// <summary>
        /// 狭路相逢选择是否参与
        /// </summary>
        /// <param name="model">提交问题模型</param>
        /// <returns></returns>
        [HttpPost]
        [Route("partner")]
        [ProducesResponseType(typeof(mJsonResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveUserExamPartner(UserPartnerViewModel model)
        {
            var json = new mJsonResult();
            try
            {
                //_logger.LogInformation("打印QuestionNumber：{0}，打印chiocePart：{1}", model.questionNumber,model.chiocePart);
                var userId = new Guid(_identityService.GetUserIdentity());
                var userPartner = _examContext.UserExamPartners
                    .FirstOrDefault(x => x.UserID == userId
                    && x.QuestionNumber == model.questionNumber);
                if (userPartner != null)
                {
                    userPartner.ChiocePart = model.chiocePart;
                    userPartner.AddTime = DateTime.Now;
                    _examContext.UserExamPartners.Update(userPartner);
                }
                else
                {
                    userPartner = new UserExamPartner
                    {
                        QuestionNumber = model.questionNumber,
                        UserID = userId,
                        ChiocePart = model.chiocePart,
                        AddTime = DateTime.Now
                    };
                    _examContext.UserExamPartners.Add(userPartner);
                }
                json.success = await _examContext.SaveChangesAsync() > 0;
            }catch(Exception ex)
            {
                _logger.LogError("狭路相逢选择是否参与错误：{0}", ex.Message);
                json.msg = "选择是否参与出错"+ex.Message;
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
        public async Task<IActionResult> GetModuleDetail(int type = 1)
        {
            ModuleDetailDto model = new ModuleDetailDto();
            var userId = _identityService.GetUserIdentity();
            var userProScore = await _examContext.UserProblemScores
                   .Where(x => x.UserID == new Guid(userId)).ToListAsync();
            var userTypeProScore = userProScore.Where(x => x.ProblemType == type);
            model.tAnswerCount = userTypeProScore.Count(c => c.Score > 0);
            model.mScore = userTypeProScore.Sum(s => s.Score);
            model.allScore = userProScore.Sum(s => s.Score);
            switch (type)
            {
                case (int)eProblemType.争分夺秒:
                    model.wAnswerCount = userTypeProScore.Count(c => c.Score < 1 || !c.Score.HasValue);
                    break;
                case (int)eProblemType.狭路相逢:
                    model.ranking = await _examContext.UserExamScores
                        .Where(o => o.TotalScores > model.allScore).CountAsync() + 1;
                    model.wAnswerCount = userTypeProScore.Count(c => c.Score < 0);
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserScores()
        {
            UserScoreDto model = new UserScoreDto();
            var userId = _identityService.GetUserIdentity();
            var userScore = await _examContext.UserExamScores.Include(c => c.User)//默认不开启懒加载
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
            else
            {
                return NotFound();
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
                .OrderByDescending(o => o.Score).LastOrDefault();
            if (userLessRanking != null)
            {
                model.xLessRanking = userLessRanking.RankingNum;
                model.xLessScore = userLessRanking.Score;
            }
            var userPlusRanking = userRankingList.Where(x => x.Score < model.xScore)
                .OrderByDescending(o => o.Score).FirstOrDefault();
            if (userPlusRanking != null)
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
        [ProducesResponseType(typeof(UserExamProcessDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetExamProcess()
        {
            UserExamProcessDto model = new UserExamProcessDto();
            var process = await _examContext.ExamProcesss.FirstOrDefaultAsync();
            if (process == null)
            {
                return NotFound();
            }
            else
            {
                model.ID = process.ID;
                model.ModuleType = process.ModuleType;
                model.Number = process.Number;
                model.SubType = process.SubType;
                model.AddTime = process.AddTime;
                if (process.ModuleType == (int)eProblemType.狭路相逢)
                {
                    var userId = new Guid(_identityService.GetUserIdentity());
                    model.ChiocePart = _examContext.UserExamPartners
                        .Where(x => x.UserID == userId&&x.QuestionNumber == process.Number)
                        .FirstOrDefault()?.ChiocePart;
                    var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();
                    var tTimeSpan = baseSetting.PartTimeSpan;
                    TimeSpan tSpan = DateTime.Now - process.AddTime;
                    if (tSpan.TotalSeconds > 0 && tTimeSpan > Convert.ToInt32(tSpan.TotalSeconds))
                    {
                        model.IsChioceOver = true;
                        model.Countdown = tTimeSpan - Convert.ToInt32(tSpan.TotalSeconds);
                    }
                    else
                    {
                        model.IsChioceOver = false;
                    }
                }
            }
            return Ok(model);
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
        /// 是否在答题区间内
        /// </summary>
        /// <param name="model">(1、狭路相逢 2、一比高下 3、狭路相逢)</param>
        /// <returns></returns>
        private async Task<bool> GetProblemInCountdown(UserProblemScore model)
        {
            bool result = false;
            var process = await _examContext.ExamProcesss
                .Where(x => x.ModuleType == model.ProblemType)
                .FirstOrDefaultAsync();
            if (process != null)
            {
                var baseSetting = await _examContext.BaseExamSettings.SingleOrDefaultAsync();

                int tTimeSpan = 0;
                switch (model.ProblemType)
                {
                    case (int)eProblemType.争分夺秒:
                        tTimeSpan = baseSetting.TypeTimeSpan1;
                        result = model.ProblemSubType == process.SubType;
                        break;
                    case (int)eProblemType.一比高下:
                        tTimeSpan = baseSetting.TypeTimeSpan2;
                        result = model.QuestionNumber == process.Number;
                        break;
                    case (int)eProblemType.狭路相逢:
                        tTimeSpan = baseSetting.TypeTimeSpan3+ baseSetting.PartTimeSpan;
                        result = model.QuestionNumber == process.Number;
                        break;
                }
                if (result)
                {
                    TimeSpan tSpan = DateTime.Now - process.AddTime;
                    if (tTimeSpan > 0 && tSpan.TotalSeconds > 0
                        && tTimeSpan+5 > Convert.ToInt32(tSpan.TotalSeconds))
                    {
                        result = tTimeSpan - Convert.ToInt32(tSpan.TotalSeconds) + 5 > 0;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}