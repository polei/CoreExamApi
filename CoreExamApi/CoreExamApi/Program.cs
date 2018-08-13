using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreExamApi.Extensions;
using CoreExamApi.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;

namespace CoreExamApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args)
            //    .MigrateDbContext<ExamContext>((context, services) =>
            //    {
            //        var logger = services.GetService<ILogger<ExamContextSeed>>();

            //        new ExamContextSeed()
            //            .SeedAsync(context, logger)
            //            .Wait();

            //    }).Run();
            //NLog.Web.NLogBuilder.ConfigureNLog("NLog.config");
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ExamContext>();
                    //ExamContextSeed.SeedAsync(context);
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
             {
                 logging.ClearProviders(); //移除已经注册的其他日志处理程序
                 logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information); //设置最小的日志级别
             })
            .UseNLog();
    }
}
