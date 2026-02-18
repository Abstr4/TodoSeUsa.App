using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Constraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ConsignorPercent",
                table: "SaleItems",
                type: "decimal(3,0)",
                precision: 3,
                scale: 0,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)",
                oldPrecision: 5,
                oldScale: 4);

            migrationBuilder.AddCheckConstraint(
                name: "CK_SaleItem_ConsignorPercent",
                table: "SaleItems",
                sql: "[ConsignorPercent] BETWEEN 0 AND 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SaleItem_ConsignorPercent",
                table: "SaleItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsignorPercent",
                table: "SaleItems",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,0)",
                oldPrecision: 3,
                oldScale: 0);
        }
    }
}
