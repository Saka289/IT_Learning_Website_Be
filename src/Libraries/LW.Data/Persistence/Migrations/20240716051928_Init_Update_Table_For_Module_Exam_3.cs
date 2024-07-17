using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_For_Module_Exam_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "Exams",
                newName: "PublicExamEssaySolutionId");

            migrationBuilder.AddColumn<string>(
                name: "PublicExamEssayId",
                table: "Exams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicExamId",
                table: "ExamCodes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicExamEssayId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "PublicExamId",
                table: "ExamCodes");

            migrationBuilder.RenameColumn(
                name: "PublicExamEssaySolutionId",
                table: "Exams",
                newName: "PublicId");
        }
    }
}
