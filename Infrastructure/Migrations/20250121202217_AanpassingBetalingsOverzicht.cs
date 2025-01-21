using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AanpassingBetalingsOverzicht : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BetalingsOverzicht_LastFor",
                table: "Orders",
                newName: "BetalingsOverzicht_Last4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BetalingsOverzicht_Last4",
                table: "Orders",
                newName: "BetalingsOverzicht_LastFor");
        }
    }
}
