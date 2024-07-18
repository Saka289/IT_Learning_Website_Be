using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_For_Module_Exam_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams");

            migrationBuilder.DropIndex(
                name: "IX_UserExams_ExamId",
                table: "UserExams");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "UserExams");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "UserExams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_ExamId",
                table: "UserExams",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }
    }
}
