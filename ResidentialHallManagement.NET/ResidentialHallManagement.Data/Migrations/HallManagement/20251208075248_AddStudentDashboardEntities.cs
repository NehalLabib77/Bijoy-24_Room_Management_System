using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddStudentDashboardEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "S_DEPARTMENT",
                table: "STUDENTS",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "S_DOB",
                table: "STUDENTS",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "S_EMAIL",
                table: "STUDENTS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_EMERGENCY_NAME",
                table: "STUDENTS",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_EMERGENCY_PHONE",
                table: "STUDENTS",
                type: "varchar(15)",
                maxLength: 15,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_EMERGENCY_RELATION",
                table: "STUDENTS",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_PHOTO_URL",
                table: "STUDENTS",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "R_BLOCK",
                table: "ROOMS",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "R_FLOOR",
                table: "ROOMS",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "R_NUMBER",
                table: "ROOMS",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "R_STATUS",
                table: "ROOMS",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ACTUAL_CHECKOUT",
                table: "ASSIGNED_ROOM",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ALLOCATION_STATUS",
                table: "ASSIGNED_ROOM",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BED_NUMBER",
                table: "ASSIGNED_ROOM",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EXPECTED_CHECKOUT",
                table: "ASSIGNED_ROOM",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MAINTENANCE_REQUESTS",
                columns: table => new
                {
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ROOM_ID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HALL_ID = table.Column<int>(type: "int", nullable: false),
                    ISSUE = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SUBMITTED_ON = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RESOLVED_ON = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TECHNICIAN_NOTE = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PRIORITY = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAINTENANCE_REQUESTS", x => x.REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_MAINTENANCE_REQUESTS_ROOMS_ROOM_ID_HALL_ID",
                        columns: x => new { x.ROOM_ID, x.HALL_ID },
                        principalTable: "ROOMS",
                        principalColumns: new[] { "R_ID", "H_ID" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MAINTENANCE_REQUESTS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PAYMENTS",
                columns: table => new
                {
                    PAYMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AMOUNT = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PAYMENT_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    METHOD = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    INVOICE_NUMBER = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DESCRIPTION = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENTS", x => x.PAYMENT_ID);
                    table.ForeignKey(
                        name: "FK_PAYMENTS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VIOLATIONS",
                columns: table => new
                {
                    VIOLATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DESCRIPTION = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ACTION_TAKEN = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SEVERITY = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RESOLVED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VIOLATIONS", x => x.VIOLATION_ID);
                    table.ForeignKey(
                        name: "FK_VIOLATIONS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MAINTENANCE_REQUESTS_ROOM_ID_HALL_ID",
                table: "MAINTENANCE_REQUESTS",
                columns: new[] { "ROOM_ID", "HALL_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_MAINTENANCE_REQUESTS_STUDENT_ID",
                table: "MAINTENANCE_REQUESTS",
                column: "STUDENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENTS_STUDENT_ID",
                table: "PAYMENTS",
                column: "STUDENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VIOLATIONS_STUDENT_ID",
                table: "VIOLATIONS",
                column: "STUDENT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MAINTENANCE_REQUESTS");

            migrationBuilder.DropTable(
                name: "PAYMENTS");

            migrationBuilder.DropTable(
                name: "VIOLATIONS");

            migrationBuilder.DropColumn(
                name: "S_DEPARTMENT",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_DOB",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_EMAIL",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_EMERGENCY_NAME",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_EMERGENCY_PHONE",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_EMERGENCY_RELATION",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "S_PHOTO_URL",
                table: "STUDENTS");

            migrationBuilder.DropColumn(
                name: "R_BLOCK",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "R_FLOOR",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "R_NUMBER",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "R_STATUS",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "ACTUAL_CHECKOUT",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropColumn(
                name: "ALLOCATION_STATUS",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropColumn(
                name: "BED_NUMBER",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropColumn(
                name: "EXPECTED_CHECKOUT",
                table: "ASSIGNED_ROOM");
        }
    }
}
