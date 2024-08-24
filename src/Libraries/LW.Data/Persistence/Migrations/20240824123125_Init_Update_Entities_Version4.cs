using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Entities_Version4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Levels",
                newName: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Levels",
                newName: "Name");
        }
    }
}
