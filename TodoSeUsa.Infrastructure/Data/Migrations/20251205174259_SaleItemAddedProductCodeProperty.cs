using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SaleItemAddedProductCodeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "SaleItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "SaleItems");
        }
    }
}
