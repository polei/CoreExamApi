using AdminWeb.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdminWeb.Services
{
    public class ExamService:IExamService
    {
        private string _connectionString = string.Empty;

        public ExamService(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<bool> InsertProblemList(List<Problem> list)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(@"insert into [dbo].[Problem]
                            ([ID],[ProblemName],[ProblemFeatures],[Answer],[Score]
                            ,[ProblemType],[ProblemSubType],[QuestionNumber])
                             values(@ID,@ProblemName,@ProblemFeatures,@Answer,@Score
                            ,@ProblemType,@ProblemSubType,@QuestionNumber)", list);
                return result>0;
            }
        }

        public async Task<List<Problem>> GetProblemList(int page, int rows, int? problemType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"with t as
                        (select *, ROW_NUMBER() OVER(Order by ProblemType,QuestionNumber) 
                        AS rownum from [dbo].[Problem] where 1=1");
                if (problemType.HasValue)
                {
                    sql.Append(" and ProblemType=@problemType");
                    Params.Add("@problemType", problemType.Value);
                }
                sql.Append(")select * from t where t.rownum between (@page-1)*@rows+1 and @page*@rows");
                Params.Add("@page", page);
                Params.Add("@rows", rows);

                connection.Open();
                var result = await connection.QueryAsync<Problem>(sql.ToString(), Params);

                return result.ToList();
            }
        }

        public async Task<int> GetScoreType(int? problemType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"select sum(Score) from [dbo].[Problem] where 1=1");
                if (problemType.HasValue)
                {
                    sql.Append(" and ProblemType=@problemType");
                    Params.Add("@problemType", problemType.Value);
                }

                connection.Open();
                var result = await connection.QueryFirstAsync<int>(sql.ToString(), Params);

                return result;
            }
        }


        public async Task<List<UserScore>> GetScoreList(int page, int rows,string searchValue)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"with t as
                        (select *, ROW_NUMBER() OVER(Order by TotalScores desc) AS rownum 
                        from [dbo].[UserExamScore])
                        select t.rownum,u.TrueName,t.TotalScores,t.TypeScores1
                        ,t.TypeScores2,t.TypeScores3 from t 
                        inner join [dbo].[User] u on t.UserID=u.ID
                        where 1=1");
                if (!string.IsNullOrEmpty(searchValue))
                {
                    sql.Append(" and u.TrueName like @searchValue");
                    Params.Add("@searchValue", "%" + searchValue + "%");
                }
                sql.Append(" and t.rownum between (@page-1)*@rows+1 and @page*@rows");
                Params.Add("@page", page);
                Params.Add("@rows", rows);
                connection.Open();
                var result = await connection.QueryAsync<UserScore>(sql.ToString(), Params);

                return result.ToList();
            }
        }

        public async Task<int> GetScoreCount(string searchValue)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"
                        select count(0) from [dbo].[UserExamScore] t");
                if (!string.IsNullOrEmpty(searchValue))
                {
                    sql.Append(@"inner join [dbo].[User] u on t.UserID=u.ID
                        where 1=1 and u.TrueName like @searchValue");
                    Params.Add("@searchValue", "%" + searchValue + "%");
                }
                connection.Open();
                var result = await connection.QueryFirstAsync<int>(sql.ToString(), Params);
                return result;
            }
        }

        public async Task<int> GetProblemCount(int? problemType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                DynamicParameters Params = new DynamicParameters();
                StringBuilder sql = new StringBuilder(@"select count(0) from [dbo].[Problem]
                        where 1=1");
                if (problemType.HasValue)
                {
                    sql.Append(" and ProblemType=@problemType");
                    Params.Add("@problemType", problemType.Value);
                }
                connection.Open();
                var result = await connection.QueryFirstAsync<int>(sql.ToString(), Params);
                return result;
            }
        }

        public async Task<bool> Issue()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                List<UserProblemScore> list = new List<UserProblemScore>();
                List<UserExamScore> userExamList = new List<UserExamScore>();
                connection.Open();
                var users =await connection.QueryAsync<User>(@"
                            select * from [dbo].[User] where UserName!='admin' 
                                and ID not in(select distinct UserID from [dbo].[UserExamScore])");
                var problems = await connection.QueryAsync<Problem>(@"
                            select * from [dbo].[Problem]");
                
                foreach (var user in users)
                {
                    var userExam = new UserExamScore();
                    userExam.ID = Guid.NewGuid();
                    userExam.UserID = user.ID;
                    userExamList.Add(userExam);
                    foreach (var problem in problems)
                    {
                        var userProblemScore = new UserProblemScore();
                        userProblemScore.ID = Guid.NewGuid();
                        userProblemScore.UserID = user.ID;
                        userProblemScore.ProblemID = problem.ID;
                        userProblemScore.ProblemName = problem.ProblemName;
                        userProblemScore.ProblemType = problem.ProblemType;
                        userProblemScore.ProblemFeatures = problem.ProblemFeatures;
                        userProblemScore.ProblemScore = problem.Score;
                        userProblemScore.Answer = problem.Answer;
                        userProblemScore.ProblemSubType = problem.ProblemSubType;
                        userProblemScore.QuestionNumber = problem.QuestionNumber;
                        list.Add(userProblemScore);
                    }
                }
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    await connection.ExecuteAsync(@"
                    insert into [dbo].[UserExamScore]([ID],[UserID],IsOver) values(@ID,@UserID,0)
                        ", userExamList, transaction);
                    await connection.ExecuteAsync(@"
                    insert into [dbo].[UserProblemScore]([ID],[ProblemID],[ProblemName]
                    ,[ProblemFeatures] ,[Answer],[ProblemScore]
                    ,[ProblemType],[ProblemSubType],[QuestionNumber],[UserID]) 
                    values(@ID,@ProblemID,@ProblemName,@ProblemFeatures,@Answer,@ProblemScore
                        ,@ProblemType,@ProblemSubType,@QuestionNumber,@UserID)
                        ", list, transaction);
                    transaction.Commit();

                }
                catch(Exception ex)
                {
                    //出现异常，事务Rollback
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                return true;

            }
                
        }

        public class UserExamScore
        {
            public Guid ID { get; set; }

            public Guid UserID { get; set; }

        }


        public async Task<bool> SaveBaseSetting(BaseSetting model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                model.ID = Guid.NewGuid();
                var result = await connection.ExecuteAsync(@"
                            insert into [dbo].[BaseExamSetting]([ID]
                            ,[TypeTimeSpan1],[TypeTimeSpan2],[TypeTimeSpan3],[PartTimeSpan]
                            ) values(@ID,@TypeTimeSpan1,@TypeTimeSpan2,@TypeTimeSpan3,@PartTimeSpan)", model);
                return result>0;
            }
        }
    }
}