using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanPhuKienDienThoai.Migrations
{
    /// <inheritdoc />
    public partial class na : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DiscountCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DiscountCodes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "DiscountCodes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DiscountCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
