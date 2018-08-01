using AdminWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWeb.Services
{
    public interface IExamService
    {
        Task<bool> InsertProblemList(List<Problem> list);

        Task<List<Problem>> GetProblemList(int page, int rows, int? problemType);

        Task<int> GetScoreType(int? problemType);

        Task<int> GetProblemCount(int? problemType);

        Task<List<UserScore>> GetScoreList(int page, int rows, string searchValue);
        Task<int> GetScoreCount(string SearchValue);

        Task<bool> Issue();

        Task<bool> SaveBaseSetting(BaseSetting model);
    }
}
