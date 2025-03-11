using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Home",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emoji = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Badges_Home_CardModelId",
                        column: x => x.CardModelId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarouselImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarouselModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarouselImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarouselImages_Home_CarouselModelId",
                        column: x => x.CarouselModelId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Home",
                columns: new[] { "Id", "Country", "Description", "Discriminator", "Image", "Title" },
                values: new object[] { 1, "South Africa", "Graduated CPUT with a Bachelors in Computer Engineering. Part-time sales Assistant at Studio 88 and Outdoor Warehouse. Software Developer Internship at 1Nebula.", "Card", "src/assets/WorkStock.jpg", "Experience Summary" });

            migrationBuilder.InsertData(
                table: "Home",
                columns: new[] { "Id", "Discriminator", "Title" },
                values: new object[] { 2, "Carousel", "HomeImages" });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "CardModelId", "Emoji", "Label" },
                values: new object[,]
                {
                    { 1, 1, "🛒", "Part-time Sales Assistant: Studio 88" },
                    { 2, 1, "🛒", "Part-time Sales Assistant: Outdoor Warehouse" },
                    { 3, 1, "💻", "Software Developer Internship: 1Nebula" },
                    { 4, 1, "🎓", "CPUT Graduate in Computer Engineering" }
                });

            migrationBuilder.InsertData(
                table: "CarouselImages",
                columns: new[] { "Id", "CarouselModelId", "ImageUrl" },
                values: new object[] { 5, 2, "[\"src/assets/profilePic.png\",\"src/assets/gintoki.png\",\"src/assets/roxas.jpg\",\"src/assets/myf.png\",\"src/assets/ffv.jpg\"]" });

            migrationBuilder.CreateIndex(
                name: "IX_Badges_CardModelId",
                table: "Badges",
                column: "CardModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarouselImages_CarouselModelId",
                table: "CarouselImages",
                column: "CarouselModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "CarouselImages");

            migrationBuilder.DropTable(
                name: "Home");
        }
    }
}
