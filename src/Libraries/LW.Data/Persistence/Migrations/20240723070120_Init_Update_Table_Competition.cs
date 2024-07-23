using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_Competition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompetitionId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CompetitionId",
                table: "Exams",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Competitions_CompetitionId",
                table: "Exams",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Competitions_CompetitionId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CompetitionId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "Exams");
        }
    }
}
