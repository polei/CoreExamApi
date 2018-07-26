﻿using CoreExamApi.Dto;
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
    }
}
