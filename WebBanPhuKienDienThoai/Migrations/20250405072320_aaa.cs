using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanPhuKienDienThoai.Migrations
{
    /// <inheritdoc />
    public partial class aaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "DiscountCodes");

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "DiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                table: "DiscountCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "DiscountCodes");

            migrationBuilder.DropColumn(
                name: "UsageLimit",
                table: "DiscountCodes");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "DiscountCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
