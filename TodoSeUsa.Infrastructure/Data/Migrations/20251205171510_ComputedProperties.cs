using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ComputedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "'TSU-' + RIGHT('0000' + CAST(Id AS VARCHAR(4)), 4)",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "People",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT_WS(' | ', EmailAddress, PhoneNumber, Address)",
                stored: false);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "People",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "[FirstName] + ' ' + [LastName]",
                stored: false);

            migrationBuilder.AddColumn<string>(
                name: "BoxCode",
                table: "Boxes",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "'BOX-' + RIGHT('000' + CAST(Id AS VARCHAR(3)), 3)",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "People");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "People");

            migrationBuilder.DropColumn(
                name: "BoxCode",
                table: "Boxes");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");
        }
    }
}
