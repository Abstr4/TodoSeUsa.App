using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NullableNotesConsignmentEntity : Migration
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

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "People",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "People",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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

            migrationBuilder.AddCheckConstraint(
                name: "CK_Person_EmailOrPhone",
                table: "People",
                sql: "[EmailAddress] IS NOT NULL OR [PhoneNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_People_EmailAddress",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_PhoneNumber",
                table: "People");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Person_EmailOrPhone",
                table: "People");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "People",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "People",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_EmailAddress",
                table: "People",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_People_PhoneNumber",
                table: "People",
                column: "PhoneNumber",
                unique: true);
        }
    }
}
