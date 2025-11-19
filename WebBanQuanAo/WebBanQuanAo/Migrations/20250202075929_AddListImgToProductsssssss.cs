using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddListImgToProductsssssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListImg",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListImg",
                table: "Products");
        }
    }
}
