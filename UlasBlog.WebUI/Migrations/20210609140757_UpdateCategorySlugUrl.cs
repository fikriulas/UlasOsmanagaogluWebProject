using Microsoft.EntityFrameworkCore.Migrations;

namespace UlasBlog.WebUI.Migrations
{
    public partial class UpdateCategorySlugUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlugUrl",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SlugUrl",
                table: "Categories",
                column: "SlugUrl",
                unique: true,
                filter: "[SlugUrl] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_SlugUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SlugUrl",
                table: "Categories");
        }
    }
}
