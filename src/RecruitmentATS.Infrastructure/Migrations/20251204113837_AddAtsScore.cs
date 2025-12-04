using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAtsScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AtsScore",
                table: "Applications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MatchDetails",
                table: "Applications",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AtsScore",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MatchDetails",
                table: "Applications");
        }
    }
}
