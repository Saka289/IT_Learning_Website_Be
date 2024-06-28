using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Topics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Documents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BookCollection",
                table: "Documents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Documents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Edition",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PublicationYear",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfBook",
                table: "Documents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "BookCollection",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Edition",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "PublicationYear",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "TypeOfBook",
                table: "Documents");
        }
    }
}
