using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreExamApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseExamSetting",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    TypeTimeSpan1 = table.Column<int>(nullable: false),
                    TypeTimeSpan2 = table.Column<int>(nullable: false),
                    TypeTimeSpan3 = table.Column<int>(nullable: false),
                    PartTimeSpan = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseExamSetting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExamProcess",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ModuleType = table.Column<int>(nullable: false),
                    SubType = table.Column<int>(nullable: true),
                    Number = table.Column<int>(nullable: true),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamProcess", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Problem",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ProblemName = table.Column<string>(maxLength: 500, nullable: true),
                    ProblemFeatures = table.Column<string>(maxLength: 500, nullable: true),
                    Answer = table.Column<string>(maxLength: 10, nullable: true),
                    Score = table.Column<decimal>(nullable: true),
                    ProblemType = table.Column<int>(nullable: false),
                    ProblemSubType = table.Column<int>(nullable: false),
                    QuestionNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 200, nullable: false),
                    TrueName = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 200, nullable: true),
                    OrderNumber = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserExamScore",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    TypeScores1 = table.Column<decimal>(nullable: true),
                    TypeScores2 = table.Column<decimal>(nullable: true),
                    TypeScores3 = table.Column<decimal>(nullable: true),
                    TotalScores = table.Column<decimal>(nullable: true),
                    IsOver = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExamScore", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserExamScore_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProblemScore",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ProblemID = table.Column<Guid>(nullable: false),
                    ProblemName = table.Column<string>(maxLength: 500, nullable: true),
                    ProblemFeatures = table.Column<string>(maxLength: 500, nullable: true),
                    Answer = table.Column<string>(maxLength: 10, nullable: true),
                    ProblemScore = table.Column<decimal>(nullable: true),
                    ProblemType = table.Column<int>(nullable: false),
                    ProblemSubType = table.Column<int>(nullable: false),
                    QuestionNumber = table.Column<int>(nullable: false),
                    ExaminationDate = table.Column<DateTime>(nullable: true),
                    SubmitAnswer = table.Column<string>(maxLength: 10, nullable: true),
                    Score = table.Column<decimal>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProblemScore", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserProblemScore_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserExamScore_UserID",
                table: "UserExamScore",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserProblemScore_UserID",
                table: "UserProblemScore",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseExamSetting");

            migrationBuilder.DropTable(
                name: "ExamProcess");

            migrationBuilder.DropTable(
                name: "Problem");

            migrationBuilder.DropTable(
                name: "UserExamScore");

            migrationBuilder.DropTable(
                name: "UserProblemScore");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
