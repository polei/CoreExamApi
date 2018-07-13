﻿using CoreExamApi.Infrastructure.EntityConfigurations;
using CoreExamApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreExamApi.Infrastructure
{
    public class ExamContext:DbContext
    {
        public ExamContext(DbContextOptions<ExamContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        public class ExamContextDesignFactory : IDesignTimeDbContextFactory<ExamContext>
        {
            public ExamContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ExamContext>()
                    .UseSqlServer("Server=.;Initial Catalog=PMSoft.ExamDB;Integrated Security=true");

                return new ExamContext(optionsBuilder.Options);
            }
        }
    }
}
