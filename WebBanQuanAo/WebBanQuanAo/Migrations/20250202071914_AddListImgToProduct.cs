using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddListImgToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListImg",
                table: "Products",
                type: "nvarchar(max)", // Lưu danh sách ảnh dưới dạng JSON
                nullable: true);
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
