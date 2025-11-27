using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexesToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create filtered unique indexes for non-deleted persons
            migrationBuilder.CreateIndex(
                name: "UX_Person_Email_NotDeleted",
                table: "People",
                column: "EmailAddress",
                unique: true,
                filter: "[DeletedAt] IS NULL AND [EmailAddress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Person_Phone_NotDeleted",
                table: "People",
                column: "PhoneNumber",
                unique: true,
                filter: "[DeletedAt] IS NULL AND [PhoneNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Person_Email_NotDeleted",
                table: "People");

            migrationBuilder.DropIndex(
                name: "UX_Person_Phone_NotDeleted",
                table: "People");
        }
    }
}