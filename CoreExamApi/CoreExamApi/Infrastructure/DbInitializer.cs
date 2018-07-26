using CoreExamApi.Models;
using Microsoft.EntityFrameworkCore;
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
            //context.Database.EnsureCreated();
            context.Database.AutoTransactionsEnabled = true;
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
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
