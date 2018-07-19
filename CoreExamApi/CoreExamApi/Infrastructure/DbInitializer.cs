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
                CompanyName = "杭州品茗",
                CreateDate = DateTime.Now,
            };
        }
    }
}
