using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddSpecificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "R_AVAILABILITY",
                table: "ROOMS",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "R_NAME",
                table: "ROOMS",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "H_LOCATION",
                table: "HALL",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ROOM_CHANGE_REQUESTS",
                columns: table => new
                {
                    REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    REQUESTED_ROOM_ID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    REQUESTED_HALL_ID = table.Column<int>(type: "int", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    REQUEST_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PROCESSED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    REASON = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_REMARKS = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOM_CHANGE_REQUESTS", x => x.REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_ROOM_CHANGE_REQUESTS_ROOMS_REQUESTED_ROOM_ID_REQUESTED_HALL_~",
                        columns: x => new { x.REQUESTED_ROOM_ID, x.REQUESTED_HALL_ID },
                        principalTable: "ROOMS",
                        principalColumns: new[] { "R_ID", "H_ID" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROOM_CHANGE_REQUESTS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_CHANGE_REQUESTS_REQUESTED_ROOM_ID_REQUESTED_HALL_ID",
                table: "ROOM_CHANGE_REQUESTS",
                columns: new[] { "REQUESTED_ROOM_ID", "REQUESTED_HALL_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_CHANGE_REQUESTS_STUDENT_ID",
                table: "ROOM_CHANGE_REQUESTS",
                column: "STUDENT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ROOM_CHANGE_REQUESTS");

            migrationBuilder.DropColumn(
                name: "R_AVAILABILITY",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "R_NAME",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "H_LOCATION",
                table: "HALL");
        }
    }
}
