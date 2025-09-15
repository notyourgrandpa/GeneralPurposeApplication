using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeneralPurposeApplication.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalkInContactNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContactNumber",
                value: "None");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContactNumber",
                value: "09096846407");
        }
    }
}
