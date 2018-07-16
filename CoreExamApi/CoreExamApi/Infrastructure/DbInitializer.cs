using CoreExamApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(ExamContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                context.Users.Add(GetAdminUser());


                context.SaveChanges();
            }
        }

        private static User GetAdminUser()
        {
            return new User
            {
                UserName = "admin",
                TrueName = "admin",
                Password = "7C4A8D09CA3762AF61E59520943DC26494F8941B",
                Sex = 1,
                CompanyName = "杭州品茗",
                Province = 0,
                City = 1,
                Address = "杭州西斗门路3天堂软件园",
                CreateDate = DateTime.Now,
                Professional = 1
            };
        }
    }
}
