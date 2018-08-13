using CoreExamApi.Dto;
using CoreExamApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Services
{
    public interface IExamService
    {
        /// <summary>
        /// 获取用户排名
        /// </summary>
        /// <param name="problemType">哪种类型的排名</param>
        /// <returns></returns>
        Task<List<UserRankingDto>> GetUserRankingList(int? problemType);

        /// <summary>
        /// 获取排名（手机端使用）
        /// </summary>
        /// <returns></returns>
        Task<List<UserRankingModel>> GetUserRankingList();

        /// <summary>
        /// 对于选择狭路相逢的选手，而没有分数的批量处理
        /// </summary>
        /// <param name="userScoreListID"></param>
        Task<bool> SumUserExamScore(List<UserProblemScoreViewModel> userScoreListID);


        /// <summary>
        /// 修改每一组提交状态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> UpdateUserProStatus(List<UserProScoreIDModel> list);
    }
}
