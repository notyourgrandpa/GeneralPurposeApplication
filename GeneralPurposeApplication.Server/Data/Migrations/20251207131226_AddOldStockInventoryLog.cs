using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPurposeApplication.Server.Data.migrations
{
    /// <inheritdoc />
    public partial class AddOldStockInventoryLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OldStock",
                table: "InventoryLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldStock",
                table: "InventoryLogs");
        }
    }
}
