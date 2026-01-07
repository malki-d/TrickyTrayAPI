using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrickyTrayAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialTicketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TicketPrices",
                columns: new[] { "Id", "Price" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TicketPrices",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
