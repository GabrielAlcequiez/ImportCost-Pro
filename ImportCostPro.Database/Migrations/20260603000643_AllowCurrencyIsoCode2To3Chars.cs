using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportCostPro.Database.Migrations
{
    /// <inheritdoc />
    public partial class AllowCurrencyIsoCode2To3Chars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Currencies_ISOCode_Length",
                table: "Currencies");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Currencies_ISOCode_Length",
                table: "Currencies",
                sql: "LEN([ISOCode]) >= 2 AND LEN([ISOCode]) <= 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Currencies_ISOCode_Length",
                table: "Currencies");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Currencies_ISOCode_Length",
                table: "Currencies",
                sql: "LEN([ISOCode]) >= 3 AND LEN([ISOCode]) <= 3");
        }
    }
}
