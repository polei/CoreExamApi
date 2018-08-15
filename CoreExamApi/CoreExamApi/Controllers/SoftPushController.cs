using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.Infrastructure.Hubs;
using CoreExamApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CoreExamApi.Controllers
{
    //[Authorize(Policy = "AdminOnly")]
    [Route("api/v1/push")]
    [ApiController]
    public class SoftPushController : ControllerBase
    {
        private readonly ExamContext _examContext;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SoftPushController(ExamContext context,
            IHubContext<NotificationsHub> hubContext)
        {
            _examContext= context ?? throw new ArgumentNullException(nameof(context));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        /// <summary>
        /// 争分夺秒
        /// </summary>
        /// <param name="subType">组（int）</param>
        /// <returns></returns>
        [HttpPost]
        [Route("all/type1/users")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendMessage(int subType=1)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.争分夺秒,subType);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", subType);
            return Ok(true);
        }

        /// <summary>
        /// 一比高下推送()
        /// </summary>
        /// <param name="questionNumber">题号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("group/type2/users")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendMessageForGroups(int questionNumber)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.一比高下, 0,questionNumber);
            var number = questionNumber - 1;
            if (number < 1)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessageFromGroup", questionNumber);
            }
            else
            {
                var userScoreList= await _examContext.UserProblemScores
                .Where(x => x.ProblemType == (int)eProblemType.一比高下
                && x.QuestionNumber == number).ToListAsync();
                List<string> groupsPass = userScoreList.Where(x => x.Score > 0)
                    .Select(s => s.UserID.ToString().ToLower()).ToList();

                List<string> groupsFailure = userScoreList
                    .Where(x => x.Score < 1)
                    .Select(s => s.UserID.ToString().ToLower()).ToList();

                #region 操作score为null的记录
                var userExamScoreList = await _examContext.UserProblemScores
                    .Where(x => x.ProblemType == (int)eProblemType.一比高下 && x.QuestionNumber < number).ToListAsync();
                var specialUserList = userScoreList.Where(x => x.Score == null).Select(s => s.UserID);
                foreach (var userID in specialUserList)
                {

                    var userProScore = userExamScoreList.Where(x => x.UserID == userID && x.Score >= 0)
                        .OrderByDescending(o => o.QuestionNumber).FirstOrDefault();
                    if (userProScore is null)
                    {
                        int[] arr = { 1, 3, 5, 7, 9 };
                        if (arr.IndexOf(questionNumber) == -1)//不存在
                        {
                            groupsFailure.Add(userID.ToString().ToLower());
                        }
                        else
                        {
                            groupsPass.Add(userID.ToString().ToLower());
                        }
                    }
                    else
                    {
                        if ((questionNumber - userProScore.QuestionNumber) % 2 == 0)
                        {
                            if (userProScore.Score > 0)//正确的时候
                            {
                                groupsFailure.Add(userID.ToString().ToLower());
                            }
                            else
                            {
                                groupsPass.Add(userID.ToString().ToLower());
                            }
                            
                        }
                        else
                        {
                            if (userProScore.Score > 0)
                            {
                                groupsPass.Add(userID.ToString().ToLower());
                            }
                            else
                            {
                                groupsFailure.Add(userID.ToString().ToLower());
                            }
                           
                        }
                    }

                }

                #endregion

                await _hubContext.Clients.Groups(groupsPass)
                .SendAsync("ReceiveMessageFromGroup", questionNumber);


                
                await _hubContext.Clients.Groups(groupsFailure)
                .SendAsync("ReceiveMessageFromGroupFailure", questionNumber);

                
            }
            //List<string> groups = new List<string>() { "567E2936-5D8A-40B0-BABA-74F4BD8CF456" };
            //var group = "567E2936-5D8A-40B0-BABA-74F4BD8CF456";
            //await _hubContext.Clients.Group(group.ToLower())
            //    .SendAsync("ReceiveMessageFromGroup", problemType, questionNumber);
            return Ok(true);
        }

        /// <summary>
        /// 狭路相逢
        /// </summary>
        /// <param name="questionNumber">题号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("all/type3/users")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendMessageForType3(int questionNumber)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.狭路相逢, 0, questionNumber);
            await _hubContext.Clients.All.SendAsync("ReceiveMessageForType3", questionNumber);
            return Ok(true);
        }

        
        //[HttpPost]
        //[Route("user")]
        //public async Task SendMessageForUsers(string userID)
        //{
        //    await _hubContext.Clients.User(userID.ToLower())
        //        .SendAsync("ReceiveMessageFromUser", userID);
        //}


        /// <summary>
        /// 结束当前的模块
        /// </summary>
        /// <param name="moduleType">moduleType（4、狭路相逢结束）</param>
        /// <returns></returns>
        [HttpPost]
        [Route("takeover/process")]
        public async Task TakeOverExamProcessAsync(int moduleType=4)
        {
            await AddOrUpdateProcessAsync(moduleType,0,0);
        }

        private async Task AddOrUpdateProcessAsync(int moduleType,int subType=0,int number=0)
        {
            var process =await _examContext.ExamProcesss.SingleOrDefaultAsync();
            if (process == null)
            {
                process = new ExamProcess();
                process.SubType = subType;
                process.ModuleType = moduleType;
                process.Number = number;
                process.AddTime = DateTime.Now;
                _examContext.Add(process);
            }
            else
            {
                process.ModuleType = moduleType;
                process.SubType = subType;
                process.Number = number;
                process.AddTime = DateTime.Now;
                _examContext.Update(process);
            }
            await _examContext.SaveChangesAsync();
        }
    }
}