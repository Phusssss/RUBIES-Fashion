using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class FixSizeColorData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sửa dữ liệu size và color bị lỗi
            migrationBuilder.Sql(@"
                UPDATE Products 
                SET AvailableSizes = 'S,M,L,XL'
                WHERE AvailableSizes = '[]' OR AvailableSizes IS NULL OR AvailableSizes = '';
                
                UPDATE Products 
                SET AvailableColors = 'Đen,Trắng,Xanh'
                WHERE AvailableColors = '[]' OR AvailableColors IS NULL OR AvailableColors = '';
                
                -- Sửa tên sản phẩm bị lặp lại
                UPDATE Products 
                SET Name = 'ĐẦM TUYTSY ĐEN CỔ VEST BUỘC DÂY EO',
                    Description = 'Đầm tuytsy đen cổ vest buộc dây eo sang trọng'
                WHERE ProductId = 1035;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
