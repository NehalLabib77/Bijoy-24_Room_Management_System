using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class InitialCreateMySQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "FEES",
                columns: table => new
                {
                    F_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    F_NAME = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_AMNT = table.Column<int>(type: "int", nullable: false),
                    F_CATEGORY = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FEES", x => x.F_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HALL",
                columns: table => new
                {
                    H_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    H_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_TYPE = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_CAPACITY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HALL", x => x.H_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MATCHES",
                columns: table => new
                {
                    M_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    M_TOURN = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    M_ROUND = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    M_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATCHES", x => x.M_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SPONSORS",
                columns: table => new
                {
                    SP_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SP_NAME = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SP_MANAGER = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SP_CONTACT = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSORS", x => x.SP_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STUDENTS",
                columns: table => new
                {
                    S_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_FATHER = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_MOTHER = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_GENDER = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_RELIGION = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_MOBILE = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_BLD_GRP = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_PRM_ADDR = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_STATUS = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENTS", x => x.S_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TEACHERS",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    T_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    T_DESIG = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEACHERS", x => x.T_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EVENT_INFO",
                columns: table => new
                {
                    E_ID = table.Column<int>(type: "int", nullable: false),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    E_INFO_SDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    E_INFO_EDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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
                name: "ROOMS",
                columns: table => new
                {
                    R_ID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    R_CAPACITY = table.Column<int>(type: "int", nullable: false),
                    R_WING = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROOMS", x => new { x.R_ID, x.H_ID });
                    table.ForeignKey(
                        name: "FK_ROOMS_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
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
                    ST_WING = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_NAME = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_AGE = table.Column<int>(type: "int", nullable: false),
                    ST_JOB_ID = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ST_H_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ST_E_DATE = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ST_SALARY = table.Column<int>(type: "int", nullable: true)
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
                name: "SPONSORSHIP",
                columns: table => new
                {
                    SP_ID = table.Column<int>(type: "int", nullable: false),
                    E_ID = table.Column<int>(type: "int", nullable: false),
                    SPON_AMNT = table.Column<int>(type: "int", nullable: true),
                    SPON_CMNT = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                name: "ASSIGNED_HALL",
                columns: table => new
                {
                    S_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    ASSIGNED_S_TYPE = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ASSIGNED_CURRENT = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true, defaultValue: "YES")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSIGNED_HALL", x => new { x.S_ID, x.H_ID });
                    table.ForeignKey(
                        name: "FK_ASSIGNED_HALL_HALL_H_ID",
                        column: x => x.H_ID,
                        principalTable: "HALL",
                        principalColumn: "H_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ASSIGNED_HALL_STUDENTS_S_ID",
                        column: x => x.S_ID,
                        principalTable: "STUDENTS",
                        principalColumn: "S_ID",
                        onDelete: ReferentialAction.Cascade);
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
                name: "DESIGNATED_HALL",
                columns: table => new
                {
                    T_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    DES_ADMIN_ROLE = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DES_ADMIN_SDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DES_ADMIN_RDATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                        name: "FK_DESIGNATED_HALL_TEACHERS_T_ID",
                        column: x => x.T_ID,
                        principalTable: "TEACHERS",
                        principalColumn: "T_ID",
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
                    table.ForeignKey(
                        name: "FK_MATCH_INFO_TEAMS_TEAM_ID",
                        column: x => x.TEAM_ID,
                        principalTable: "TEAMS",
                        principalColumn: "TEAM_ID",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_TEAM_MEMBER_TEAMS_TEAM_ID",
                        column: x => x.TEAM_ID,
                        principalTable: "TEAMS",
                        principalColumn: "TEAM_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ASSIGNED_ROOM",
                columns: table => new
                {
                    ASS_ROOM_SID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    S_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ASS_HALL_H_ID = table.Column<int>(type: "int", nullable: false),
                    R_H_ID = table.Column<int>(type: "int", nullable: false),
                    R_ID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ASS_ROOM_SDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ASS_ROOM_EDATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSIGNED_ROOM", x => x.ASS_ROOM_SID);
                    table.ForeignKey(
                        name: "FK_ASSIGNED_ROOM_ASSIGNED_HALL_S_ID_ASS_HALL_H_ID",
                        columns: x => new { x.S_ID, x.ASS_HALL_H_ID },
                        principalTable: "ASSIGNED_HALL",
                        principalColumns: new[] { "S_ID", "H_ID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_R_H_ID",
                        columns: x => new { x.R_ID, x.R_H_ID },
                        principalTable: "ROOMS",
                        principalColumns: new[] { "R_ID", "H_ID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ASSIGNED_ROOM_STUDENTS_S_ID",
                        column: x => x.S_ID,
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
                    S_ID = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    H_ID = table.Column<int>(type: "int", nullable: false),
                    F_ID = table.Column<int>(type: "int", nullable: false),
                    S_F_MONTH = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_F_YEAR = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    S_F_LDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    S_F_PDATE = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                name: "IX_ASSIGNED_HALL_H_ID",
                table: "ASSIGNED_HALL",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_R_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "R_H_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNED_ROOM_S_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "S_ID", "ASS_HALL_H_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_DESIGNATED_HALL_H_ID",
                table: "DESIGNATED_HALL",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENT_INFO_H_ID",
                table: "EVENT_INFO",
                column: "H_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MATCH_INFO_M_ID",
                table: "MATCH_INFO",
                column: "M_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PARTICIPATION_E_ID",
                table: "PARTICIPATION",
                column: "E_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ROOMS_H_ID",
                table: "ROOMS",
                column: "H_ID");

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
                name: "IX_TEAM_MEMBER_S_ID",
                table: "TEAM_MEMBER",
                column: "S_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TEAMS_H_ID",
                table: "TEAMS",
                column: "H_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASSIGNED_ROOM");

            migrationBuilder.DropTable(
                name: "DESIGNATED_HALL");

            migrationBuilder.DropTable(
                name: "EVENT_INFO");

            migrationBuilder.DropTable(
                name: "MATCH_INFO");

            migrationBuilder.DropTable(
                name: "PARTICIPATION");

            migrationBuilder.DropTable(
                name: "SPONSORSHIP");

            migrationBuilder.DropTable(
                name: "STAFFS");

            migrationBuilder.DropTable(
                name: "STUDENTS_FEES");

            migrationBuilder.DropTable(
                name: "TEAM_MEMBER");

            migrationBuilder.DropTable(
                name: "ROOMS");

            migrationBuilder.DropTable(
                name: "TEACHERS");

            migrationBuilder.DropTable(
                name: "MATCHES");

            migrationBuilder.DropTable(
                name: "EVENTS");

            migrationBuilder.DropTable(
                name: "SPONSORS");

            migrationBuilder.DropTable(
                name: "ASSIGNED_HALL");

            migrationBuilder.DropTable(
                name: "FEES");

            migrationBuilder.DropTable(
                name: "TEAMS");

            migrationBuilder.DropTable(
                name: "STUDENTS");

            migrationBuilder.DropTable(
                name: "HALL");
        }
    }
}
