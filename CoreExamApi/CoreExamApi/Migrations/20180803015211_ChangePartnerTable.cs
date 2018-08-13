using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreExamApi.Migrations
{
    public partial class ChangePartnerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChiocePart",
                table: "UserExamPartner",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiocePart",
                table: "UserExamPartner");
        }
    }
}
