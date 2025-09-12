using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPurposeApplication.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedWalkInCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "ContactNumber", "Email", "Name" },
                values: new object[] { 1, null, null, null, "Walk-in" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
