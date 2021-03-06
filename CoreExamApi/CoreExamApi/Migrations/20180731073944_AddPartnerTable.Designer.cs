﻿// <auto-generated />
using System;
using CoreExamApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreExamApi.Migrations
{
    [DbContext(typeof(ExamContext))]
    [Migration("20180731073944_AddPartnerTable")]
    partial class AddPartnerTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoreExamApi.Models.BaseExamSetting", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PartTimeSpan");

                    b.Property<int>("TypeTimeSpan1");

                    b.Property<int>("TypeTimeSpan2");

                    b.Property<int>("TypeTimeSpan3");

                    b.HasKey("ID");

                    b.ToTable("BaseExamSetting");
                });

            modelBuilder.Entity("CoreExamApi.Models.ExamProcess", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddTime");

                    b.Property<int>("ModuleType");

                    b.Property<int?>("Number");

                    b.Property<int?>("SubType");

                    b.HasKey("ID");

                    b.ToTable("ExamProcess");
                });

            modelBuilder.Entity("CoreExamApi.Models.Problem", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Answer")
                        .HasMaxLength(10);

                    b.Property<string>("ProblemFeatures")
                        .HasMaxLength(500);

                    b.Property<string>("ProblemName")
                        .HasMaxLength(500);

                    b.Property<int>("ProblemSubType");

                    b.Property<int>("ProblemType");

                    b.Property<int>("QuestionNumber");

                    b.Property<decimal?>("Score");

                    b.HasKey("ID");

                    b.ToTable("Problem");
                });

            modelBuilder.Entity("CoreExamApi.Models.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CompanyName")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("OrderNumber");

                    b.Property<string>("TrueName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CoreExamApi.Models.UserExamPartner", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AddTime");

                    b.Property<int>("QuestionNumber");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.ToTable("UserExamPartner");
                });

            modelBuilder.Entity("CoreExamApi.Models.UserExamScore", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsOver");

                    b.Property<decimal?>("TotalScores");

                    b.Property<decimal?>("TypeScores1");

                    b.Property<decimal?>("TypeScores2");

                    b.Property<decimal?>("TypeScores3");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("UserExamScore");
                });

            modelBuilder.Entity("CoreExamApi.Models.UserProblemScore", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Answer")
                        .HasMaxLength(10);

                    b.Property<DateTime?>("ExaminationDate");

                    b.Property<string>("ProblemFeatures")
                        .HasMaxLength(500);

                    b.Property<Guid>("ProblemID");

                    b.Property<string>("ProblemName")
                        .HasMaxLength(500);

                    b.Property<decimal?>("ProblemScore");

                    b.Property<int>("ProblemSubType");

                    b.Property<int>("ProblemType");

                    b.Property<int>("QuestionNumber");

                    b.Property<decimal?>("Score");

                    b.Property<string>("SubmitAnswer")
                        .HasMaxLength(10);

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("UserProblemScore");
                });

            modelBuilder.Entity("CoreExamApi.Models.UserExamScore", b =>
                {
                    b.HasOne("CoreExamApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreExamApi.Models.UserProblemScore", b =>
                {
                    b.HasOne("CoreExamApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
