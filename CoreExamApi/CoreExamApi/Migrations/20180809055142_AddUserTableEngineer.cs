using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreExamApi.Migrations
{
    public partial class AddUserTableEngineer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsEngineer",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEngineer",
                table: "User");
        }
    }
}
