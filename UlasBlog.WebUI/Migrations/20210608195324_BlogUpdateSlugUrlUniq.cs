using Microsoft.EntityFrameworkCore.Migrations;

namespace UlasBlog.WebUI.Migrations
{
    public partial class BlogUpdateSlugUrlUniq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SlugUrl",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_SlugUrl",
                table: "Blogs",
                column: "SlugUrl",
                unique: true,
                filter: "[SlugUrl] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Blogs_SlugUrl",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "SlugUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
