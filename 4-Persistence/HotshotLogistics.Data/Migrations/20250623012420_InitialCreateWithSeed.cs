using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotshotLogistics.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PickupAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DropoffAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedDeliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDriverId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Drivers_AssignedDriverId",
                        column: x => x.AssignedDriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "JobAssignments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    JobId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAssignments_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobAssignments_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "LicenseExpiryDate", "LicenseNumber", "PhoneNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "alice.smith@example.com", "Alice", true, "Smith", new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "A1234567", "555-1234", null },
                    { 2, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "bob.johnson@example.com", "Bob", true, "Johnson", new DateTime(2031, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "B7654321", "555-5678", null },
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Amount", "AssignedDriverId", "CreatedAt", "DropoffAddress", "EstimatedDeliveryTime", "PickupAddress", "Priority", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { "job-1", 100.00m, 1, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "456 Elm St", "2024-06-16T10:00:00Z", "123 Main St", "High", "Pending", "Deliver Package A", null },
                    { "job-2", 75.50m, 2, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "321 Pine St", "2024-06-17T14:00:00Z", "789 Oak St", "Medium", "InTransit", "Deliver Package B", null },
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAssignments_AssignedAt",
                table: "JobAssignments",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_JobAssignments_DriverId",
                table: "JobAssignments",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAssignments_JobId",
                table: "JobAssignments",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAssignments_Status",
                table: "JobAssignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AssignedDriverId",
                table: "Jobs",
                column: "AssignedDriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAssignments");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
