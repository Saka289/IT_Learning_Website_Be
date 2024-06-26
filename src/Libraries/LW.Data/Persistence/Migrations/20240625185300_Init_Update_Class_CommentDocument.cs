using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Update_Class_CommentDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "CommentDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentDocuments_ParentId",
                table: "CommentDocuments",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments",
                column: "ParentId",
                principalTable: "CommentDocuments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments");

            migrationBuilder.DropIndex(
                name: "IX_CommentDocuments_ParentId",
                table: "CommentDocuments");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CommentDocuments");
        }
    }
}
