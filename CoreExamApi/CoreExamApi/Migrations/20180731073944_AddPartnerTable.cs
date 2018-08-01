using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreExamApi.Migrations
{
    public partial class AddPartnerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserExamPartner",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    QuestionNumber = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExamPartner", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserExamPartner");
        }
    }
}
