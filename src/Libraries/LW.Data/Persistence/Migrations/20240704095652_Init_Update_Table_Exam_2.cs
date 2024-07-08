using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Table_Exam_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Exams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Exams");
        }
    }
}
