using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UlasBlog.WebUI.Migrations
{
    public partial class addSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    SiteDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SmtpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Port = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    MailUserName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    MailPassword = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
