using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Class_QuizQuestion_version4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionRelations_QuizQuestions_QUizQuestionId",
                table: "QuizQuestionRelations");

            migrationBuilder.RenameColumn(
                name: "QUizQuestionId",
                table: "QuizQuestionRelations",
                newName: "QuizQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionRelations_QUizQuestionId",
                table: "QuizQuestionRelations",
                newName: "IX_QuizQuestionRelations_QuizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionRelations_QuizQuestions_QuizQuestionId",
                table: "QuizQuestionRelations",
                column: "QuizQuestionId",
                principalTable: "QuizQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestionRelations_QuizQuestions_QuizQuestionId",
                table: "QuizQuestionRelations");

            migrationBuilder.RenameColumn(
                name: "QuizQuestionId",
                table: "QuizQuestionRelations",
                newName: "QUizQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestionRelations_QuizQuestionId",
                table: "QuizQuestionRelations",
                newName: "IX_QuizQuestionRelations_QUizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestionRelations_QuizQuestions_QUizQuestionId",
                table: "QuizQuestionRelations",
                column: "QUizQuestionId",
                principalTable: "QuizQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
