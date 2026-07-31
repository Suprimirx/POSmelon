using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "CashRegisterSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CashierName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InitialCashAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalSales = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalCashSales = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalTransferSales = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalCash = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalMoney = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InvoiceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Observations = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisterSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BarCode = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    GenericName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Concentration = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Presentation = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    InvimaRegistration = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SalePrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IVAPercentage = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CashierName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CashRegisterNumber = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CashReceived = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Base = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalIVA = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AnnulmentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CashRegisterSessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_CashRegisterSessions_CashRegisterSessionId",
                        column: x => x.CashRegisterSessionId,
                        principalTable: "CashRegisterSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    BarCode = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IvaPercentage = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SubTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Base = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalIVA = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterSessions_OpeningDate",
                table: "CashRegisterSessions",
                column: "OpeningDate");

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterSessions_State",
                table: "CashRegisterSessions",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_InvoiceId",
                table: "InvoiceLines",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_ProductId",
                table: "InvoiceLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CashRegisterSessionId",
                table: "Invoices",
                column: "CashRegisterSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Date",
                table: "Invoices",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_State",
                table: "Invoices",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Active",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "BarCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandName",
                table: "Products",
                column: "BrandName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_GenericName",
                table: "Products",
                column: "GenericName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "CashRegisterSessions");
        }
    }
}
