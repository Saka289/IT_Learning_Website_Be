using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Entities_Version1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Problems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_GradeId",
                table: "Quizzes",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_GradeId",
                table: "Problems",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Grades_GradeId",
                table: "Problems",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Grades_GradeId",
                table: "Quizzes",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Grades_GradeId",
                table: "Problems");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Grades_GradeId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_GradeId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Problems_GradeId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Problems");
        }
    }
}
