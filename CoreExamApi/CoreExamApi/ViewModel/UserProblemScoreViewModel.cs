using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.ViewModel
{
    public class UserProblemScoreViewModel
    {
        public Guid userProblemScoreID { get; set; }

        public Guid UserID { get; set; }

        public decimal? ProblemScore { get; set; }

        public decimal? TypeScores3 { get; set; }

        public decimal? TotalScores { get; set; }
    }
}
