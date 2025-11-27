using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailAndPhoneIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_People_EmailAddress",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_PhoneNumber",
                table: "People");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_People_EmailAddress",
                table: "People",
                column: "EmailAddress",
                unique: true,
                filter: "[EmailAddress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_People_PhoneNumber",
                table: "People",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }
    }
}