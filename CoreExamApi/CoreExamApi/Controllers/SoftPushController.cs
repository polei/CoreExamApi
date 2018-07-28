using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreExamApi.Infrastructure;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.Infrastructure.Hubs;
using CoreExamApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CoreExamApi.Controllers
{
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
        public async Task SendMessage(int subType=1)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.争分夺秒,subType);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", subType);
        }

        /// <summary>
        /// 一比高下推送()
        /// </summary>
        /// <param name="questionNumber">题号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("group/type2/users")]
        public async Task SendMessageForGroups(int questionNumber)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.一比高下, 0,questionNumber);
            var number = questionNumber - 1;
            if (number < 1)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessageFromGroup");
            }
            else
            {
                List<string> groups = await _examContext.UserProblemScores
                .Where(x => x.ProblemType == (int)eProblemType.一比高下 
                && x.QuestionNumber == number && x.Score > 0)
                .Select(s => s.UserID.ToString().ToLower()).ToListAsync();

                await _hubContext.Clients.Groups(groups)
                .SendAsync("ReceiveMessageFromGroup", questionNumber);
            }
            //List<string> groups = new List<string>() { "567E2936-5D8A-40B0-BABA-74F4BD8CF456" };
            //var group = "567E2936-5D8A-40B0-BABA-74F4BD8CF456";
            //await _hubContext.Clients.Group(group.ToLower())
            //    .SendAsync("ReceiveMessageFromGroup", problemType, questionNumber);
        }

        /// <summary>
        /// 狭路相逢
        /// </summary>
        /// <param name="questionNumber">题号</param>
        /// <returns></returns>
        [HttpPost]
        [Route("all/type3/users")]
        public async Task SendMessageForType3(int questionNumber)
        {
            await AddOrUpdateProcessAsync((int)eProblemType.争分夺秒, 0, questionNumber);
            await _hubContext.Clients.All.SendAsync("ReceiveMessageForType3", questionNumber);
        }

        //[HttpPost]
        //[Route("user")]
        //public async Task SendMessageForUsers(string userID)
        //{
        //    await _hubContext.Clients.User(userID.ToLower())
        //        .SendAsync("ReceiveMessageFromUser", userID);
        //}


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