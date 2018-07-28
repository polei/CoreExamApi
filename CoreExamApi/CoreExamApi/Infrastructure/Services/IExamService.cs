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
    }
}
