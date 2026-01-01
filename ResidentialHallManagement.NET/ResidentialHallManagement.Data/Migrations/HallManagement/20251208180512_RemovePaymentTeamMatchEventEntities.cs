using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class RemovePaymentTeamMatchEventEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DESIGNATED_HALL");

            migrationBuilder.DropTable(
                name: "EVENTS");

            migrationBuilder.DropTable(
                name: "MATCH_INFO");

            migrationBuilder.DropTable(
                name: "PAYMENTS");

            migrationBuilder.DropTable(
                name: "TEAM_MEMBER");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropTable(
                name: "MATCHES");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENTS",
                columns: table => new
                {
                    E_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    E_NAME = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTS", x => x.E_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MATCHES",
                columns: table => new
                {
                    M_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    M_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    M_ROUND = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    M_TOURN = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATCHES", x => x.M_ID);
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
                    CREATED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    INVOICE_NUMBER = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    METHOD = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PAYMENT_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                name: "Teacher",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    T_DESIG = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    T_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.T_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TEAM_MEMBER",
                columns: table => new
                {
                    TEAM_ID = table.Column<int>(type: "int", nullable: false),
                    S_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    T_MEM_ROLE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAM_MEMBER", x => new { x.TEAM_ID, x.S_ID });
                    table.ForeignKey(
                        name: "FK_TEAM_MEMBER_STUDENTS_S_ID",
                        column: x => x.S_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MATCH_INFO",
                columns: table => new
                {
                    TEAM_ID = table.Column<int>(type: "int", nullable: false),
                    M_ID = table.Column<int>(type: "int", nullable: false),
                    M_INFO_RESULT = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    M_INFO_WINNER = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATCH_INFO", x => new { x.TEAM_ID, x.M_ID });
                    table.ForeignKey(
                        name: "FK_MATCH_INFO_MATCHES_M_ID",
                        column: x => x.M_ID,
                        principalTable: "MATCHES",
                        principalColumn: "M_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DESIGNATED_HALL",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    DES_ADMIN_ROLE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DES_ADMIN_RDATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DES_ADMIN_SDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DESIGNATED_HALL", x => new { x.T_ID, x.H_ID });
                    table.ForeignKey(
                        name: "FK_DESIGNATED_HALL_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DESIGNATED_HALL_Teacher_T_ID",
                        column: x => x.T_ID,
                        principalTable: "Teacher",
                        principalColumn: "T_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DESIGNATED_HALL_H_ID",
                table: "DESIGNATED_HALL",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MATCH_INFO_M_ID",
                table: "MATCH_INFO",
                column: "M_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENTS_STUDENT_ID",
                table: "PAYMENTS",
                column: "STUDENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TEAM_MEMBER_S_ID",
                table: "TEAM_MEMBER",
                column: "S_ID");
        }
    }
}
