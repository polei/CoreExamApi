using AdminWeb.Models;
using PMSoft.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWeb.Services
{
    public interface IUserService
    {
        Task<dynamic> GetUserFromLogin(string userName, string password);

        UserInfo GetUserInfoByUserName(string userName);

        Task<bool> InsertUserList(List<User> userList);

        Task<List<User>> GetUserList(int page, int rows);

        Task<int> GetUserCount();

        Task<List<User>> GetUserInfoList();
    }
}
