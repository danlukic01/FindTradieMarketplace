using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindTradie.Services.TradieManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradieProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ABN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    ACN = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ServiceRadius = table.Column<double>(type: "float", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    IsEmergencyService = table.Column<bool>(type: "bit", nullable: false),
                    InsuranceProvider = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InsuranceExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradieProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradieProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BeforeImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AfterImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ProjectValue = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_TradieProfiles_TradieProfileId",
                        column: x => x.TradieProfileId,
                        principalTable: "TradieProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradieAvailability",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradieProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsEmergencyAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradieAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradieAvailability_TradieProfiles_TradieProfileId",
                        column: x => x.TradieProfileId,
                        principalTable: "TradieProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradieDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradieProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerificationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradieDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradieDocuments_TradieProfiles_TradieProfileId",
                        column: x => x.TradieProfileId,
                        principalTable: "TradieProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradieLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradieProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Suburb = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 8, nullable: false),
                    Longitude = table.Column<double>(type: "float(11)", precision: 11, scale: 8, nullable: false),
                    IsPrimaryLocation = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradieLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradieLocations_TradieProfiles_TradieProfileId",
                        column: x => x.TradieProfileId,
                        principalTable: "TradieProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradieServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradieProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MinPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    MaxPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradieServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradieServices_TradieProfiles_TradieProfileId",
                        column: x => x.TradieProfileId,
                        principalTable: "TradieProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_Category",
                table: "PortfolioItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_CompletionDate",
                table: "PortfolioItems",
                column: "CompletionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_IsPublic",
                table: "PortfolioItems",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_TradieProfileId",
                table: "PortfolioItems",
                column: "TradieProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TradieAvailability_TradieProfileId_DayOfWeek",
                table: "TradieAvailability",
                columns: new[] { "TradieProfileId", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradieDocuments_DocumentType",
                table: "TradieDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_TradieDocuments_TradieProfileId",
                table: "TradieDocuments",
                column: "TradieProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TradieDocuments_VerificationStatus",
                table: "TradieDocuments",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TradieLocations_Latitude_Longitude",
                table: "TradieLocations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_TradieLocations_PostCode",
                table: "TradieLocations",
                column: "PostCode");

            migrationBuilder.CreateIndex(
                name: "IX_TradieLocations_TradieProfileId",
                table: "TradieLocations",
                column: "TradieProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TradieProfiles_ABN",
                table: "TradieProfiles",
                column: "ABN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradieProfiles_IsAvailable",
                table: "TradieProfiles",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_TradieProfiles_Rating",
                table: "TradieProfiles",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_TradieProfiles_UserId",
                table: "TradieProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradieProfiles_VerificationStatus",
                table: "TradieProfiles",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TradieServices_Category",
                table: "TradieServices",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_TradieServices_IsActive",
                table: "TradieServices",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TradieServices_TradieProfileId",
                table: "TradieServices",
                column: "TradieProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioItems");

            migrationBuilder.DropTable(
                name: "TradieAvailability");

            migrationBuilder.DropTable(
                name: "TradieDocuments");

            migrationBuilder.DropTable(
                name: "TradieLocations");

            migrationBuilder.DropTable(
                name: "TradieServices");

            migrationBuilder.DropTable(
                name: "TradieProfiles");
        }
    }
}
