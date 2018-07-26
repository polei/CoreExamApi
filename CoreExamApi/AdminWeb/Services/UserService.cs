using AdminWeb.Models;
using Dapper;
using PMSoft.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AdminWeb.Services
{
    public class UserService : IUserService
    {
        private string _connectionString = string.Empty;

        public UserService(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<dynamic> GetUserFromLogin(string userName, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryFirstAsync<dynamic>(@"select * from [dbo].[User] 
                                where UserName=@userName and UserName=@password", new { userName, password });
                return result;
            }
        }

        public UserInfo GetUserInfoByUserName(string userName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = connection.QueryFirst<dynamic>(@"select * from [dbo].[User] 
                                where UserName=@userName", new { userName });

                return MapUserInfo(result);
            }
        }


        public async Task<bool> InsertUserList(List<User> userList)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result =await connection.ExecuteAsync(@"insert into [dbo].[User]
                            (ID,UserName,TrueName,CompanyName,OrderNumber,CreateDate)
                        values(@ID,@UserName,@TrueName,@CompanyName,@OrderNumber,@CreateDate)", userList);

                return result>0;
            }
        }

        public async Task<List<User>> GetUserList(int page, int rows)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(@"with t as
                        (select *, ROW_NUMBER() OVER(Order by UserName ) AS rownum from [dbo].[User] where UserName!='admin')
                        select * from t where t.rownum between (@page-1)*@rows and @page*@rows",new { page,rows });

                return result.ToList();
            }
        }

        public async Task<int> GetUserCount()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryFirstAsync<int>(@"
                            select count(0) from [dbo].[User] where UserName!='admin'");
                return result;
            }
        }

        public async Task<List<User>> GetUserInfoList()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<User>(@"
                            select * from [dbo].[User] where UserName!='admin'");
                return result.ToList();
            }
        }

        private UserInfo MapUserInfo(dynamic result)
        {
            return new UserInfo
            {
                UserId = result.ID,
                UserName = result.UserName
            };
        }
    }
}