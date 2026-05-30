using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportCostPro.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.CheckConstraint("CK_Countries_ISOCode_Length", "LEN([ISOCode]) >= 2 AND LEN([ISOCode]) <= 3");
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISOCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocalCurrency = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.CheckConstraint("CK_Currencies_ISOCode_Length", "LEN([ISOCode]) >= 3 AND LEN([ISOCode]) <= 3");
                });

            migrationBuilder.CreateTable(
                name: "TariffCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TariffCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TariffPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ApplyITBIS = table.Column<bool>(type: "bit", nullable: false),
                    ApplyExciseTax = table.Column<bool>(type: "bit", nullable: false),
                    ExciseTaxPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneralItbisPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CustomsServiceFeePercentage = table.Column<decimal>(type: "decimal(5,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Importers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importers", x => x.Id);
                    table.CheckConstraint("CK_Importers_CountryId_NotEmpty", "[CountryId] <> '00000000-0000-0000-0000-000000000000'");
                    table.ForeignKey(
                        name: "FK_Importers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.CheckConstraint("CK_ExchangeRate_Value_Positive", "RateValue > 0");
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_SourceCurrencyId",
                        column: x => x.SourceCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_TargetCurrencyId",
                        column: x => x.TargetCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suppliers_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CodeReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TariffCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_TariffCategories_TariffCategoryId",
                        column: x => x.TariffCategoryId,
                        principalTable: "TariffCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransportMode = table.Column<int>(type: "int", nullable: false),
                    ImporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExchangeRateValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportOrders_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportOrders_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportOrders_Importers_ImporterId",
                        column: x => x.ImporterId,
                        principalTable: "Importers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitCostFob = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CustomDutyPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DesiredProfitMargin = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_ImportOrders_ImportOrderId",
                        column: x => x.ImportOrderId,
                        principalTable: "ImportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportOrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImportOrderExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRateValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ApportionmentMethod = table.Column<int>(type: "int", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportOrderExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportOrderExpenses_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImportOrderExpenses_ImportOrders_ImportOrderId",
                        column: x => x.ImportOrderId,
                        principalTable: "ImportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandedCostCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocalCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExchangeRateUsed = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GeneralItbisPercentageUsed = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CustomsServiceFeePercentageUsed = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FobTotalOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FobTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreightTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CifTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DutyTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectiveTaxTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomsServiceFeeTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItbisTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocalExpensesTotalLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportTotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandedCostCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandedCostCalculations_Currencies_LocalCurrencyId",
                        column: x => x.LocalCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LandedCostCalculations_ImportOrders_ImportOrderId",
                        column: x => x.ImportOrderId,
                        principalTable: "ImportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LandedCostCalculationDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LandedCostCalculationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TariffPercentageUsed = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ExciseTaxPercentageUsed = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FobOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FobLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreightAssigned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceAssigned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CifLocal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DutyCalculated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectiveTaxCalculated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomsServiceFeeCalculated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItbisCalculated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocalExpensesAssigned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportTotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitLandedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DesiredProfitMargin = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SuggestedSellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandedCostCalculationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LandedCostCalculationDetails_LandedCostCalculations_LandedCostCalculationId",
                        column: x => x.LandedCostCalculationId,
                        principalTable: "LandedCostCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandedCostCalculationDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ISOCode",
                table: "Countries",
                column: "ISOCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsLocalCurrency",
                table: "Currencies",
                column: "IsLocalCurrency",
                unique: true,
                filter: "[IsLocalCurrency] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_ISOCode",
                table: "Currencies",
                column: "ISOCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_SourceCurrencyId",
                table: "ExchangeRates",
                column: "SourceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_TargetCurrencyId",
                table: "ExchangeRates",
                column: "TargetCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Importers_CountryId",
                table: "Importers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Importers_TaxId",
                table: "Importers",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_ImportOrderId_ProductId",
                table: "ImportOrderDetails",
                columns: new[] { "ImportOrderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderDetails_ProductId",
                table: "ImportOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderExpenses_CurrencyId",
                table: "ImportOrderExpenses",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrderExpenses_ImportOrderId",
                table: "ImportOrderExpenses",
                column: "ImportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CountryId",
                table: "ImportOrders",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_CurrencyId",
                table: "ImportOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_ImporterId",
                table: "ImportOrders",
                column: "ImporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_OrderNumber",
                table: "ImportOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportOrders_SupplierId",
                table: "ImportOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostCalculationDetails_LandedCostCalculationId",
                table: "LandedCostCalculationDetails",
                column: "LandedCostCalculationId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostCalculationDetails_ProductId",
                table: "LandedCostCalculationDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostCalculations_ImportOrderId",
                table: "LandedCostCalculations",
                column: "ImportOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandedCostCalculations_LocalCurrencyId",
                table: "LandedCostCalculations",
                column: "LocalCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CodeReference",
                table: "Products",
                column: "CodeReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CountryId",
                table: "Products",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TariffCategoryId",
                table: "Products",
                column: "TariffCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CountryId",
                table: "Suppliers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CurrencyId",
                table: "Suppliers",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffCategories_TariffCode",
                table: "TariffCategories",
                column: "TariffCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "ImportOrderDetails");

            migrationBuilder.DropTable(
                name: "ImportOrderExpenses");

            migrationBuilder.DropTable(
                name: "LandedCostCalculationDetails");

            migrationBuilder.DropTable(
                name: "TaxConfigurations");

            migrationBuilder.DropTable(
                name: "LandedCostCalculations");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ImportOrders");

            migrationBuilder.DropTable(
                name: "TariffCategories");

            migrationBuilder.DropTable(
                name: "Importers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
