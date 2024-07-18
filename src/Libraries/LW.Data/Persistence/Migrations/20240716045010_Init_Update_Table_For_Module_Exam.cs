using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_For_Module_Exam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamAnswers_Exams_ExamId",
                table: "ExamAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams");

            migrationBuilder.DropTable(
                name: "ExamImages");

            migrationBuilder.RenameColumn(
                name: "ExamFile",
                table: "Exams",
                newName: "ExamSolutionFile");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "ExamAnswers",
                newName: "ExamCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamAnswers_ExamId",
                table: "ExamAnswers",
                newName: "IX_ExamAnswers_ExamCodeId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "UserExams",
                type: "decimal(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<int>(
                name: "ExamId",
                table: "UserExams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ExamCodeId",
                table: "UserExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExamEssayFile",
                table: "Exams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExamCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamFile = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamCodes_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_ExamCodeId",
                table: "UserExams",
                column: "ExamCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCodes_ExamId",
                table: "ExamCodes",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamAnswers_ExamCodes_ExamCodeId",
                table: "ExamAnswers",
                column: "ExamCodeId",
                principalTable: "ExamCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExams_ExamCodes_ExamCodeId",
                table: "UserExams",
                column: "ExamCodeId",
                principalTable: "ExamCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamAnswers_ExamCodes_ExamCodeId",
                table: "ExamAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExams_ExamCodes_ExamCodeId",
                table: "UserExams");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams");

            migrationBuilder.DropTable(
                name: "ExamCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserExams_ExamCodeId",
                table: "UserExams");

            migrationBuilder.DropColumn(
                name: "ExamCodeId",
                table: "UserExams");

            migrationBuilder.DropColumn(
                name: "ExamEssayFile",
                table: "Exams");

            migrationBuilder.RenameColumn(
                name: "ExamSolutionFile",
                table: "Exams",
                newName: "ExamFile");

            migrationBuilder.RenameColumn(
                name: "ExamCodeId",
                table: "ExamAnswers",
                newName: "ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamAnswers_ExamCodeId",
                table: "ExamAnswers",
                newName: "IX_ExamAnswers_ExamId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "UserExams",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AlterColumn<int>(
                name: "ExamId",
                table: "UserExams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ExamImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Index = table.Column<int>(type: "int", nullable: false),
                    publicId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamImages_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ExamImages_ExamId",
                table: "ExamImages",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamAnswers_Exams_ExamId",
                table: "ExamAnswers",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExams_Exams_ExamId",
                table: "UserExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
