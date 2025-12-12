using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoSeUsa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consignments_Providers_ProviderId",
                table: "Consignments");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanedProducts_LoanNotes_LoanNoteId",
                table: "LoanedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Consignments_ConsignmentId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Consignments_Providers_ProviderId",
                table: "Consignments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanedProducts_LoanNotes_LoanNoteId",
                table: "LoanedProducts",
                column: "LoanNoteId",
                principalTable: "LoanNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products",
                column: "BoxId",
                principalTable: "Boxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Consignments_ConsignmentId",
                table: "Products",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consignments_Providers_ProviderId",
                table: "Consignments");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanedProducts_LoanNotes_LoanNoteId",
                table: "LoanedProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Consignments_ConsignmentId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Consignments_Providers_ProviderId",
                table: "Consignments",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanedProducts_LoanNotes_LoanNoteId",
                table: "LoanedProducts",
                column: "LoanNoteId",
                principalTable: "LoanNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Boxes_BoxId",
                table: "Products",
                column: "BoxId",
                principalTable: "Boxes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Consignments_ConsignmentId",
                table: "Products",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Sales_SaleId",
                table: "SaleItems",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
