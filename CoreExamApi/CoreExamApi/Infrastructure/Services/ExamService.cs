﻿using CoreExamApi.Dto;
using CoreExamApi.Infrastructure.Enum;
using CoreExamApi.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure.Services
{
    public class ExamService:IExamService
    {
        private string _connectionString = string.Empty;

        public ExamService(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }


        /// <summary>
        /// 获取用户排名
        /// </summary>
        /// <param name="problemType">哪种类型的排名</param>
        /// <returns></returns>
        public async Task<List<UserRankingDto>> GetUserRankingList(int? problemType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"select rank() over (order by ");
                if (problemType.HasValue)
                {
                    switch (problemType.Value)
                    {
                        case (int)eProblemType.争分夺秒:
                            sql.Append(@"TypeScores1 desc)as RankingNum,a.TypeScores1 as Score");
                            break;
                        case (int)eProblemType.一比高下:
                            sql.Append(@"TypeScores2 desc)as RankingNum,a.TypeScores2 as Score");
                            break;
                        case (int)eProblemType.狭路相逢:
                            sql.Append(@"TypeScores3 desc)as RankingNum,a.TypeScores3 as Score");
                            break;
                        default:
                            sql.Append(@"TotalScores desc)as RankingNum,a.TotalScores as Score");
                            break;
                    }
                }
                else
                {
                    sql.Append(@"TotalScores desc)as RankingNum,a.TotalScores as Score");
                }
                sql.Append(@",b.TrueName,b.CompanyName from [dbo].[UserExamScore] a
                                  inner join [dbo].[User] b on a.UserID = b.ID");
                connection.Open();
                var result = await connection.QueryAsync<UserRankingDto>(sql.ToString());
                return result.ToList();
            }
        }

        /// <summary>
        /// 获取排名（手机端使用）
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserRankingModel>> GetUserRankingList()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<UserRankingModel>(@"
                        select rank() over (order by TotalScores desc)as RankingNum
                        ,UserID,TotalScores as Score 
                        from [dbo].[UserExamScore]");
                return result.ToList();
            }
        }

        /// <summary>
        /// 对于选择狭路相逢的选手，而没有分数的批量处理
        /// </summary>
        /// <param name="userScoreListID"></param>
        /// <returns></returns>
        public async Task<bool> SumUserExamScore(List<UserProblemScoreViewModel> userScoreListID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(@"
                        Update [dbo].[UserProblemScore] set Score=-a.ProblemScore 
                        from [dbo].[UserProblemScore] a where ID=@userProblemScoreID;
                        Update [dbo].[UserExamScore] set TypeScores3=@TypeScores3,TotalScores=@TotalScores 
                        from [dbo].[UserExamScore] where UserID=@UserID", userScoreListID);
                return result>0;
            }
        }

        /// <summary>
        /// 修改每一组提交状态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserProStatus(List<UserProScoreIDModel> list)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(@"
                        Update [dbo].[UserProblemScore] set IsSubmitOver=1 
                from [dbo].[UserProblemScore] a where ID=@scoreID;
                        ", list);
                return result > 0;
            }
        }
    }
}
