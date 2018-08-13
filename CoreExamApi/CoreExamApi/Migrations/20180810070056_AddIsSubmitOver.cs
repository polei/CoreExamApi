using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreExamApi.Migrations
{
    public partial class AddIsSubmitOver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsSubmitOver",
                table: "UserProblemScore",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubmitOver",
                table: "UserProblemScore");
        }
    }
}
