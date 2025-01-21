using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrdersAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BestellingsDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KoperEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeverAddress_Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeverAddress_Straat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeverAddress_Toevoeging = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeverAddress_Plaats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeverAddress_Provincie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeverAddress_Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeverAddress_Land = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeveringsMethodeId = table.Column<int>(type: "int", nullable: false),
                    BetalingsOverzicht_LastFor = table.Column<int>(type: "int", nullable: false),
                    BetalingsOverzicht_Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BetalingsOverzicht_ExpMonth = table.Column<int>(type: "int", nullable: false),
                    BetalingsOverzicht_Year = table.Column<int>(type: "int", nullable: false),
                    Subtotaal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BestellingsStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BetalingsIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_DeliveryMethods_LeveringsMethodeId",
                        column: x => x.LeveringsMethodeId,
                        principalTable: "DeliveryMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemBesteld_ProductId = table.Column<int>(type: "int", nullable: false),
                    ItemBesteld_ProductNaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemBesteld_FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prijs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Hoeveelheid = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_LeveringsMethodeId",
                table: "Orders",
                column: "LeveringsMethodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
