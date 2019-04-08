using Microsoft.EntityFrameworkCore.Migrations;

namespace IReckonUAssignment.DataAccessLayer.Migrations
{
    public partial class CreateArticleDBSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    ArtikelCode = table.Column<string>(nullable: true),
                    ColorCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    DiscountPrice = table.Column<decimal>(nullable: false),
                    DeliveredIn = table.Column<string>(nullable: true),
                    Q1 = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
