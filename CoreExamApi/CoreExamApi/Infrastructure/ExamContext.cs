using CoreExamApi.Infrastructure.EntityConfigurations;
using CoreExamApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure
{
    public class ExamContext : DbContext
    {
        public ExamContext(DbContextOptions<ExamContext> options) : base(options)
        { }

        public DbSet<BaseExamSetting> BaseExamSettings { get; set; }
        public DbSet<ExamProcess> ExamProcesss { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<UserExamScore> UserExamScores { get; set; }
        public DbSet<UserProblemScore> UserProblemScores { get; set; }

        //public class ExamContextDesignFactory : IDesignTimeDbContextFactory<ExamContext>
        //{
        //    public ExamContext CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<ExamContext>()
        //            .UseSqlServer("Server=.;Initial Catalog=PMSoft.ExamDB;Integrated Security=true");

        //        return new ExamContext(optionsBuilder.Options);
        //    }
        //}
    }
}
