using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_Exam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_GradeId",
                table: "Exams",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Grades_GradeId",
                table: "Exams",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Grades_GradeId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_GradeId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Exams");
        }
    }
}
