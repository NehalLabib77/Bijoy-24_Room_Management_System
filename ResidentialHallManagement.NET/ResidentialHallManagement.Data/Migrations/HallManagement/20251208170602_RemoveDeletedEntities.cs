using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class RemoveDeletedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MATCH_INFO_TEAMS_TEAM_ID",
                table: "MATCH_INFO");

            migrationBuilder.DropForeignKey(
                name: "FK_TEAM_MEMBER_TEAMS_TEAM_ID",
                table: "TEAM_MEMBER");

            migrationBuilder.DropTable(
                name: "EVENT_INFO");

            migrationBuilder.DropTable(
                name: "PARTICIPATION");

            migrationBuilder.DropTable(
                name: "SPONSORSHIP");

            migrationBuilder.DropTable(
                name: "STAFFS");

            migrationBuilder.DropTable(
                name: "STUDENTS_FEES");

            migrationBuilder.DropTable(
                name: "TEAMS");

            migrationBuilder.DropTable(
                name: "VIOLATIONS");

            migrationBuilder.DropTable(
                name: "FEES");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENT_INFO",
                columns: table => new
                {
                    E_ID = table.Column<int>(type: "int", nullable: false),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    E_INFO_EDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    E_INFO_SDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENT_INFO", x => new { x.E_ID, x.H_ID });
                    table.ForeignKey(
                        name: "FK_EVENT_INFO_EVENTS_E_ID",
                        column: x => x.E_ID,
                        principalTable: "EVENTS",
                        principalColumn: "E_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVENT_INFO_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FEES",
                columns: table => new
                {
                    F_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    F_CATEGORY = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_AMNT = table.Column<int>(type: "int", nullable: false),
                    F_NAME = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FEES", x => x.F_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PARTICIPATION",
                columns: table => new
                {
                    S_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    E_ID = table.Column<int>(type: "int", nullable: false),
                    E_ROLE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PARTICIPATION", x => new { x.S_ID, x.E_ID });
                    table.ForeignKey(
                        name: "FK_PARTICIPATION_EVENTS_E_ID",
                        column: x => x.E_ID,
                        principalTable: "EVENTS",
                        principalColumn: "E_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PARTICIPATION_STUDENTS_S_ID",
                        column: x => x.S_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SPONSORSHIP",
                columns: table => new
                {
                    SP_ID = table.Column<int>(type: "int", nullable: false),
                    E_ID = table.Column<int>(type: "int", nullable: false),
                    SPON_CMNT = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SPON_AMNT = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSORSHIP", x => new { x.SP_ID, x.E_ID });
                    table.ForeignKey(
                        name: "FK_SPONSORSHIP_EVENTS_E_ID",
                        column: x => x.E_ID,
                        principalTable: "EVENTS",
                        principalColumn: "E_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPONSORSHIP_SPONSORS_SP_ID",
                        column: x => x.SP_ID,
                        principalTable: "SPONSORS",
                        principalColumn: "SP_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STAFFS",
                columns: table => new
                {
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    ST_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_AGE = table.Column<int>(type: "int", nullable: false),
                    ST_E_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ST_H_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ST_JOB_ID = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_SALARY = table.Column<int>(type: "int", nullable: true),
                    ST_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_WING = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STAFFS", x => new { x.H_ID, x.ST_ID });
                    table.ForeignKey(
                        name: "FK_STAFFS_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TEAMS",
                columns: table => new
                {
                    TEAM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    TEAM_SPORT = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAMS", x => x.TEAM_ID);
                    table.ForeignKey(
                        name: "FK_TEAMS_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
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
                    ACTION_TAKEN = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RESOLVED_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SEVERITY = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STATUS = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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

            migrationBuilder.CreateTable(
                name: "STUDENTS_FEES",
                columns: table => new
                {
                    ST_FEE_MEMO_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    F_ID = table.Column<int>(type: "int", nullable: false),
                    S_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    S_F_LDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    S_F_MONTH = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_F_PDATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    S_F_YEAR = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENTS_FEES", x => x.ST_FEE_MEMO_ID);
                    table.ForeignKey(
                        name: "FK_STUDENTS_FEES_ASSIGNED_HALL_S_ID_H_ID",
                        columns: x => new { x.S_ID, x.H_ID },
                        principalTable: "ASSIGNED_HALL",
                        principalColumns: new[] { "S_ID", "H_ID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STUDENTS_FEES_FEES_F_ID",
                        column: x => x.F_ID,
                        principalTable: "FEES",
                        principalColumn: "F_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENTS_FEES_STUDENTS_S_ID",
                        column: x => x.S_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EVENT_INFO_H_ID",
                table: "EVENT_INFO",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PARTICIPATION_E_ID",
                table: "PARTICIPATION",
                column: "E_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSORSHIP_E_ID",
                table: "SPONSORSHIP",
                column: "E_ID");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENTS_FEES_F_ID",
                table: "STUDENTS_FEES",
                column: "F_ID");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENTS_FEES_S_ID_H_ID",
                table: "STUDENTS_FEES",
                columns: new[] { "S_ID", "H_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMS_H_ID",
                table: "TEAMS",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VIOLATIONS_STUDENT_ID",
                table: "VIOLATIONS",
                column: "STUDENT_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_MATCH_INFO_TEAMS_TEAM_ID",
                table: "MATCH_INFO",
                column: "TEAM_ID",
                principalTable: "TEAMS",
                principalColumn: "TEAM_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TEAM_MEMBER_TEAMS_TEAM_ID",
                table: "TEAM_MEMBER",
                column: "TEAM_ID",
                principalTable: "TEAMS",
                principalColumn: "TEAM_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
