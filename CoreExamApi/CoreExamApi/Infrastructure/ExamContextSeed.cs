using CoreExamApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure
{
    public class ExamContextSeed
    {
        public async Task SeedAsync(ExamContext context, ILogger<ExamContextSeed> logger, int retries = 3)
        {
            var policy = CreatePolicy(retries, logger, nameof(ExamContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                if (!context.Users.Any())
                {
                    await context.Users.AddAsync(
                        GetAdminUser());

                    await context.SaveChangesAsync();
                }
            });

        }

        private User GetAdminUser()
        {
            return new User
            {
                UserName = "admin",
                TrueName="admin",
                Password= "7C4A8D09CA3762AF61E59520943DC26494F8941B",
                Sex=1,
                CompanyName="杭州品茗",
                Province=0,
                City=1,
                Address="杭州西斗门路3天堂软件园",
                CreateDate=DateTime.Now,
                Professional=1
            };
        }


        private Policy CreatePolicy(int retries, ILogger<ExamContextSeed> logger, string prefix)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
