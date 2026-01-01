using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    public partial class AddBoarderRegistryAndRoomAvailableSlots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boarder_registry",
                columns: table => new
                {
                    BOARDER_NO = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    NAME = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    STATUS = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, defaultValue: "Available"),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boarder_registry", x => x.BOARDER_NO);
                });

            // Add S_BOARDER_NO column to students table
            migrationBuilder.AddColumn<string>(
                name: "S_BOARDER_NO",
                table: "students",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            // Add R_AVAILABLE column to rooms table
            migrationBuilder.AddColumn<int>(
                name: "R_AVAILABLE",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Initialize R_AVAILABLE to R_CAPACITY for existing rows
            migrationBuilder.Sql("UPDATE rooms SET R_AVAILABLE = R_CAPACITY WHERE R_AVAILABLE = 0;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "boarder_registry");
            migrationBuilder.DropColumn(name: "S_BOARDER_NO", table: "students");
            migrationBuilder.DropColumn(name: "R_AVAILABLE", table: "rooms");
        }
    }
}
