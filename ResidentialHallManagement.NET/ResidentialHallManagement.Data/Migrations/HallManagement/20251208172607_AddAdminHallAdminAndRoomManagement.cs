using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddAdminHallAdminAndRoomManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DESIGNATED_HALL_TEACHERS_T_ID",
                table: "DESIGNATED_HALL");

            migrationBuilder.DropTable(
                name: "SPONSORS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TEACHERS",
                table: "TEACHERS");

            migrationBuilder.RenameTable(
                name: "TEACHERS",
                newName: "Teacher");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher",
                column: "T_ID");

            migrationBuilder.CreateTable(
                name: "ADMINS",
                columns: table => new
                {
                    ADMIN_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_ID = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PHONE = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ROLE = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MODIFIED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    STATUS = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADMINS", x => x.ADMIN_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HALL_ADMINS",
                columns: table => new
                {
                    HALL_ADMIN_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HALL_ID = table.Column<int>(type: "int", nullable: false),
                    USER_ID = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_NAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EMAIL = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PHONE = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_ROLE = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    START_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    STATUS = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HALL_ADMINS", x => x.HALL_ADMIN_ID);
                    table.ForeignKey(
                        name: "FK_HALL_ADMINS_HALL_HALL_ID",
                        column: x => x.HALL_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ROOM_APPLICATION_REQUESTS",
                columns: table => new
                {
                    APPLICATION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HALL_ID = table.Column<int>(type: "int", nullable: false),
                    REQUESTED_ROOM_IDENTITY = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    APPLICATION_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ADMIN_REMARKS = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    APPROVAL_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    APPROVED_BY = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOM_APPLICATION_REQUESTS", x => x.APPLICATION_ID);
                    table.ForeignKey(
                        name: "FK_ROOM_APPLICATION_REQUESTS_HALL_HALL_ID",
                        column: x => x.HALL_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROOM_APPLICATION_REQUESTS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ROOM_ASSIGNMENTS",
                columns: table => new
                {
                    ASSIGNMENT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ROOM_IDENTITY = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HALL_ID = table.Column<int>(type: "int", nullable: false),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BED_NUMBER = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ASSIGNMENT_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EXPECTED_CHECKOUT = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ACTUAL_CHECKOUT = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NOTES = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOM_ASSIGNMENTS", x => x.ASSIGNMENT_ID);
                    table.ForeignKey(
                        name: "FK_ROOM_ASSIGNMENTS_HALL_HALL_ID",
                        column: x => x.HALL_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ROOM_ASSIGNMENTS_STUDENTS_STUDENT_ID",
                        column: x => x.STUDENT_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HALL_ADMINS_HALL_ID",
                table: "HALL_ADMINS",
                column: "HALL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_APPLICATION_REQUESTS_HALL_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                column: "HALL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_APPLICATION_REQUESTS_STUDENT_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                column: "STUDENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_ASSIGNMENTS_HALL_ID",
                table: "ROOM_ASSIGNMENTS",
                column: "HALL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOM_ASSIGNMENTS_STUDENT_ID",
                table: "ROOM_ASSIGNMENTS",
                column: "STUDENT_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DESIGNATED_HALL_Teacher_T_ID",
                table: "DESIGNATED_HALL",
                column: "T_ID",
                principalTable: "Teacher",
                principalColumn: "T_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DESIGNATED_HALL_Teacher_T_ID",
                table: "DESIGNATED_HALL");

            migrationBuilder.DropTable(
                name: "ADMINS");

            migrationBuilder.DropTable(
                name: "HALL_ADMINS");

            migrationBuilder.DropTable(
                name: "ROOM_APPLICATION_REQUESTS");

            migrationBuilder.DropTable(
                name: "ROOM_ASSIGNMENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher");

            migrationBuilder.RenameTable(
                name: "Teacher",
                newName: "TEACHERS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TEACHERS",
                table: "TEACHERS",
                column: "T_ID");

            migrationBuilder.CreateTable(
                name: "SPONSORS",
                columns: table => new
                {
                    SP_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SP_CONTACT = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SP_MANAGER = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SP_NAME = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSORS", x => x.SP_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_DESIGNATED_HALL_TEACHERS_T_ID",
                table: "DESIGNATED_HALL",
                column: "T_ID",
                principalTable: "TEACHERS",
                principalColumn: "T_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
