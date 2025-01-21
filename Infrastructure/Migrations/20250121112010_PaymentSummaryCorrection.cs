using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentSummaryCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BetalingsOverzicht_Year",
                table: "Orders",
                newName: "BetalingsOverzicht_ExpYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BetalingsOverzicht_ExpYear",
                table: "Orders",
                newName: "BetalingsOverzicht_Year");
        }
    }
}
