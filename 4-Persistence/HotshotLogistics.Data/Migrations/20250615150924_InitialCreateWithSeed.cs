// <copyright file="20250615150924_InitialCreateWithSeed.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotshotLogistics.Data.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class InitialCreateWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LicenseNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickupAddress = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DropoffAddress = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedDeliveryTime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDriverId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "IX_Jobs_AssignedDriverId",
                table: "Jobs",
                column: "AssignedDriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
