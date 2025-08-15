using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindTradie.Services.JobManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_CascadeLoop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Quotes_AcceptedQuoteId",
                table: "Jobs");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Jobs",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(11)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Jobs",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(10)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Quotes_AcceptedQuoteId",
                table: "Jobs",
                column: "AcceptedQuoteId",
                principalTable: "Quotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Quotes_AcceptedQuoteId",
                table: "Jobs");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Jobs",
                type: "float(11)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Jobs",
                type: "float(10)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Quotes_AcceptedQuoteId",
                table: "Jobs",
                column: "AcceptedQuoteId",
                principalTable: "Quotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
