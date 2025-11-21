using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSizeColorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cập nhật size và color mặc định cho các sản phẩm
            migrationBuilder.Sql(@"
                UPDATE Products 
                SET AvailableSizes = 'S,M,L,XL'
                WHERE AvailableSizes IS NULL OR AvailableSizes = '';
                
                UPDATE Products 
                SET AvailableColors = 'Đen,Trắng,Xanh'
                WHERE AvailableColors IS NULL OR AvailableColors = '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
