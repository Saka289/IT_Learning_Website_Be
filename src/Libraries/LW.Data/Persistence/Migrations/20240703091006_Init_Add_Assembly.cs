using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LW.Data.Persistence.Migrations
{
    public partial class Init_Add_Assembly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Topics_ParentId",
                table: "Topics");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments",
                column: "ParentId",
                principalTable: "CommentDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Topics_ParentId",
                table: "Topics",
                column: "ParentId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Topics_ParentId",
                table: "Topics");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDocuments_CommentDocuments_ParentId",
                table: "CommentDocuments",
                column: "ParentId",
                principalTable: "CommentDocuments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Topics_ParentId",
                table: "Topics",
                column: "ParentId",
                principalTable: "Topics",
                principalColumn: "Id");
        }
    }
}
