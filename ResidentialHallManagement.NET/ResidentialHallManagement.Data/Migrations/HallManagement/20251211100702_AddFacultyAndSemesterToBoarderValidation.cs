using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddFacultyAndSemesterToBoarderValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, update empty BoarderNo values with temporary unique placeholders
            migrationBuilder.Sql(@"
                UPDATE students 
                SET S_BOARDER_NO = CONCAT('TEMP_', S_ID) 
                WHERE S_BOARDER_NO IS NULL OR S_BOARDER_NO = '';
            ");

            migrationBuilder.AddColumn<string>(
                name: "S_FACULTY",
                table: "students",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Engineering")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "S_SEMESTER",
                table: "students",
                type: "int",
                nullable: false,
                defaultValue: 1);

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
        }
    }
}
