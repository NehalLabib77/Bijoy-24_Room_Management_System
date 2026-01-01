using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddAdminUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "S_DOB",
                table: "students");

            migrationBuilder.DropColumn(
                name: "S_EMERGENCY_RELATION",
                table: "students");

            migrationBuilder.DropColumn(
                name: "S_PHOTO_URL",
                table: "students");

            migrationBuilder.RenameColumn(
                name: "S_MOTHER",
                table: "students",
                newName: "S_MOTHER_NAME");

            migrationBuilder.RenameColumn(
                name: "S_FATHER",
                table: "students",
                newName: "S_FATHER_NAME");

            migrationBuilder.AlterColumn<string>(
                name: "S_RELIGION",
                table: "students",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldMaxLength: 8)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "S_DEPARTMENT",
                table: "students",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "S_MOTHER_NAME",
                table: "students",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldMaxLength: 25)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "S_FATHER_NAME",
                table: "students",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldMaxLength: 25)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_FACULTY",
                table: "students",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "S_SEMESTER",
                table: "students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "admin_users",
                columns: table => new
                {
                    ADMIN_USER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USERNAME = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PASSWORD_HASH = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ROLE = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CREATED_AT = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_users", x => x.ADMIN_USER_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_students_S_BOARDER_NO",
                table: "students",
                column: "S_BOARDER_NO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_boarder_registry_STUDENT_ID",
                table: "boarder_registry",
                column: "STUDENT_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_boarder_registry_students_STUDENT_ID",
                table: "boarder_registry",
                column: "STUDENT_ID",
                principalTable: "students",
                principalColumn: "S_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_students_boarder_registry_S_BOARDER_NO",
                table: "students",
                column: "S_BOARDER_NO",
                principalTable: "boarder_registry",
                principalColumn: "BOARDER_NO",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boarder_registry_students_STUDENT_ID",
                table: "boarder_registry");

            migrationBuilder.DropForeignKey(
                name: "FK_students_boarder_registry_S_BOARDER_NO",
                table: "students");

            migrationBuilder.DropTable(
                name: "admin_users");

            migrationBuilder.DropIndex(
                name: "IX_students_S_BOARDER_NO",
                table: "students");

            migrationBuilder.DropIndex(
                name: "IX_boarder_registry_STUDENT_ID",
                table: "boarder_registry");

            migrationBuilder.DropColumn(
                name: "S_FACULTY",
                table: "students");

            migrationBuilder.DropColumn(
                name: "S_SEMESTER",
                table: "students");

            migrationBuilder.RenameColumn(
                name: "S_MOTHER_NAME",
                table: "students",
                newName: "S_MOTHER");

            migrationBuilder.RenameColumn(
                name: "S_FATHER_NAME",
                table: "students",
                newName: "S_FATHER");

            migrationBuilder.UpdateData(
                table: "students",
                keyColumn: "S_RELIGION",
                keyValue: null,
                column: "S_RELIGION",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "S_RELIGION",
                table: "students",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "S_DEPARTMENT",
                table: "students",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "students",
                keyColumn: "S_MOTHER",
                keyValue: null,
                column: "S_MOTHER",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "S_MOTHER",
                table: "students",
                type: "varchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "students",
                keyColumn: "S_FATHER",
                keyValue: null,
                column: "S_FATHER",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "S_FATHER",
                table: "students",
                type: "varchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "S_DOB",
                table: "students",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "S_EMERGENCY_RELATION",
                table: "students",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "S_PHOTO_URL",
                table: "students",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
