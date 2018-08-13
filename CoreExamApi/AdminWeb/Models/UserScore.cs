using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWeb.Models
{
    public class UserScore
    {
        public int rownum { get; set; }
        public int RankingNum { get; set; }
        public string TrueName { get; set; }

        public decimal? TotalScores { get; set; }
        public decimal? TypeScores1 { get; set; }
        
        public decimal? TypeScores2 { get; set; }
        
        public decimal? TypeScores3 { get; set; }


        public string UserName { get; set; }

        public int OrderNumber { get; set; }

        public string CompanyName { get; set; }

        public int IsEngineer { get; set; }


    }
}