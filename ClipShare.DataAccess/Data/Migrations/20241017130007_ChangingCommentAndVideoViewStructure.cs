using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClipShare.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangingCommentAndVideoViewStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "VideoView",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VideoView_AppUserId",
                table: "VideoView",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AppUserId",
                table: "Comment",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView");

            migrationBuilder.DropIndex(
                name: "IX_VideoView_AppUserId",
                table: "VideoView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AppUserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView",
                columns: new[] { "AppUserId", "VideoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "AppUserId", "VideoId" });
        }
    }
}
