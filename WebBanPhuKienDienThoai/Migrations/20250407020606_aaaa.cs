using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanPhuKienDienThoai.Migrations
{
    /// <inheritdoc />
    public partial class aaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_OrderId",
                table: "ProductImages",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Orders_OrderId",
                table: "ProductImages",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Orders_OrderId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_OrderId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Orders");
        }
    }
}
