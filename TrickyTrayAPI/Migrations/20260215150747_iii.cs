using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrickyTrayAPI.Migrations
{
    /// <inheritdoc />
    public partial class iii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Gifts_GiftId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GiftId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GiftId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "GiftId1",
                table: "PurchaseItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_GiftId1",
                table: "PurchaseItems",
                column: "GiftId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Gifts_GiftId1",
                table: "PurchaseItems",
                column: "GiftId1",
                principalTable: "Gifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Gifts_GiftId1",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_GiftId1",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "GiftId1",
                table: "PurchaseItems");

            migrationBuilder.AddColumn<int>(
                name: "GiftId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GiftId",
                table: "Users",
                column: "GiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Gifts_GiftId",
                table: "Users",
                column: "GiftId",
                principalTable: "Gifts",
                principalColumn: "Id");
        }
    }
}
