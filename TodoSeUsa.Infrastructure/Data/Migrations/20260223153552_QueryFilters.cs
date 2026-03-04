using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class QueryFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Consignments");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Sales",
                newName: "PublicId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_Code",
                table: "Sales",
                newName: "IX_Sales_PublicId");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Payouts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Consignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_PublicId",
                table: "Payouts",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_PublicId",
                table: "Consignments",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payouts_PublicId",
                table: "Payouts");

            migrationBuilder.DropIndex(
                name: "IX_Consignments_PublicId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Payouts");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Consignments");

            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "Sales",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_PublicId",
                table: "Sales",
                newName: "IX_Sales_Code");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Consignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
