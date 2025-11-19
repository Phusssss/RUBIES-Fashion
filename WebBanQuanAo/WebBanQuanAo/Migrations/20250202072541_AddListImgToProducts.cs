using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddListImgToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListImg",
                table: "Products",
                newName: "ListImgJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListImgJson",
                table: "Products",
                newName: "ListImg");
        }
    }
}
