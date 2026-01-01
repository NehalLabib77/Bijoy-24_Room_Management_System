using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class AddBoarderRegistryAndRoomSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_HALL_HALL_H_ID",
                table: "ASSIGNED_HALL");

            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_HALL_STUDENTS_S_ID",
                table: "ASSIGNED_HALL");

            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_ROOM_ASSIGNED_HALL_S_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_ROOM_STUDENTS_S_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropForeignKey(
                name: "FK_HALL_ADMINS_HALL_HALL_ID",
                table: "HALL_ADMINS");

            migrationBuilder.DropForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_ROOMS_ROOM_ID_HALL_ID",
                table: "MAINTENANCE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_STUDENTS_STUDENT_ID",
                table: "MAINTENANCE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_APPLICATION_REQUESTS_HALL_HALL_ID",
                table: "ROOM_APPLICATION_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_APPLICATION_REQUESTS_STUDENTS_STUDENT_ID",
                table: "ROOM_APPLICATION_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_ASSIGNMENTS_HALL_HALL_ID",
                table: "ROOM_ASSIGNMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_ASSIGNMENTS_STUDENTS_STUDENT_ID",
                table: "ROOM_ASSIGNMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_ROOMS_REQUESTED_ROOM_ID_REQUESTED_HALL_~",
                table: "ROOM_CHANGE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_STUDENTS_STUDENT_ID",
                table: "ROOM_CHANGE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOMS_HALL_H_ID",
                table: "ROOMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_STUDENTS",
                table: "STUDENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ROOMS",
                table: "ROOMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ROOM_ASSIGNMENTS",
                table: "ROOM_ASSIGNMENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ROOM_APPLICATION_REQUESTS",
                table: "ROOM_APPLICATION_REQUESTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HALL_ADMINS",
                table: "HALL_ADMINS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HALL",
                table: "HALL");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ASSIGNED_ROOM",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ASSIGNED_HALL",
                table: "ASSIGNED_HALL");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ADMINS",
                table: "ADMINS");

            migrationBuilder.RenameTable(
                name: "STUDENTS",
                newName: "students");

            migrationBuilder.RenameTable(
                name: "ROOMS",
                newName: "rooms");

            migrationBuilder.RenameTable(
                name: "ROOM_ASSIGNMENTS",
                newName: "room_assignments");

            migrationBuilder.RenameTable(
                name: "ROOM_APPLICATION_REQUESTS",
                newName: "room_application_requests");

            migrationBuilder.RenameTable(
                name: "HALL_ADMINS",
                newName: "hall_admins");

            migrationBuilder.RenameTable(
                name: "HALL",
                newName: "hall");

            migrationBuilder.RenameTable(
                name: "ASSIGNED_ROOM",
                newName: "assigned_room");

            migrationBuilder.RenameTable(
                name: "ASSIGNED_HALL",
                newName: "assigned_hall");

            migrationBuilder.RenameTable(
                name: "ADMINS",
                newName: "admins");

            migrationBuilder.RenameIndex(
                name: "IX_ROOMS_H_ID",
                table: "rooms",
                newName: "IX_rooms_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ROOM_ASSIGNMENTS_STUDENT_ID",
                table: "room_assignments",
                newName: "IX_room_assignments_STUDENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ROOM_ASSIGNMENTS_HALL_ID",
                table: "room_assignments",
                newName: "IX_room_assignments_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ROOM_APPLICATION_REQUESTS_STUDENT_ID",
                table: "room_application_requests",
                newName: "IX_room_application_requests_STUDENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ROOM_APPLICATION_REQUESTS_HALL_ID",
                table: "room_application_requests",
                newName: "IX_room_application_requests_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_HALL_ADMINS_HALL_ID",
                table: "hall_admins",
                newName: "IX_hall_admins_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ASSIGNED_ROOM_S_ID_ASS_HALL_H_ID",
                table: "assigned_room",
                newName: "IX_assigned_room_S_ID_ASS_HALL_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_ASS_HALL_H_ID",
                table: "assigned_room",
                newName: "IX_assigned_room_R_ID_ASS_HALL_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ASSIGNED_HALL_H_ID",
                table: "assigned_hall",
                newName: "IX_assigned_hall_H_ID");

            migrationBuilder.AddColumn<string>(
                name: "S_BOARDER_NO",
                table: "students",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "R_AVAILABLE",
                table: "rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_students",
                table: "students",
                column: "S_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rooms",
                table: "rooms",
                columns: new[] { "R_ID", "H_ID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_room_assignments",
                table: "room_assignments",
                column: "ASSIGNMENT_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_room_application_requests",
                table: "room_application_requests",
                column: "APPLICATION_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_hall_admins",
                table: "hall_admins",
                column: "HALL_ADMIN_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_hall",
                table: "hall",
                column: "H_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_assigned_room",
                table: "assigned_room",
                column: "ASS_ROOM_SID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_assigned_hall",
                table: "assigned_hall",
                columns: new[] { "S_ID", "H_ID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_admins",
                table: "admins",
                column: "ADMIN_ID");

            migrationBuilder.CreateTable(
                name: "boarder_registry",
                columns: table => new
                {
                    BOARDER_NO = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NAME = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STATUS = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    STUDENT_ID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boarder_registry", x => x.BOARDER_NO);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_hall_hall_H_ID",
                table: "assigned_hall",
                column: "H_ID",
                principalTable: "hall",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_hall_students_S_ID",
                table: "assigned_hall",
                column: "S_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_room_assigned_hall_S_ID_ASS_HALL_H_ID",
                table: "assigned_room",
                columns: new[] { "S_ID", "ASS_HALL_H_ID" },
                principalTable: "assigned_hall",
                principalColumns: new[] { "S_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_room_rooms_R_ID_ASS_HALL_H_ID",
                table: "assigned_room",
                columns: new[] { "R_ID", "ASS_HALL_H_ID" },
                principalTable: "rooms",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_assigned_room_students_S_ID",
                table: "assigned_room",
                column: "S_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_hall_admins_hall_HALL_ID",
                table: "hall_admins",
                column: "HALL_ID",
                principalTable: "hall",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_rooms_ROOM_ID_HALL_ID",
                table: "MAINTENANCE_REQUESTS",
                columns: new[] { "ROOM_ID", "HALL_ID" },
                principalTable: "rooms",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_students_STUDENT_ID",
                table: "MAINTENANCE_REQUESTS",
                column: "STUDENT_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_application_requests_hall_HALL_ID",
                table: "room_application_requests",
                column: "HALL_ID",
                principalTable: "hall",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_application_requests_students_STUDENT_ID",
                table: "room_application_requests",
                column: "STUDENT_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_assignments_hall_HALL_ID",
                table: "room_assignments",
                column: "HALL_ID",
                principalTable: "hall",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_assignments_students_STUDENT_ID",
                table: "room_assignments",
                column: "STUDENT_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_rooms_REQUESTED_ROOM_ID_REQUESTED_HALL_~",
                table: "ROOM_CHANGE_REQUESTS",
                columns: new[] { "REQUESTED_ROOM_ID", "REQUESTED_HALL_ID" },
                principalTable: "rooms",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_students_STUDENT_ID",
                table: "ROOM_CHANGE_REQUESTS",
                column: "STUDENT_ID",
                principalTable: "students",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_hall_H_ID",
                table: "rooms",
                column: "H_ID",
                principalTable: "hall",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assigned_hall_hall_H_ID",
                table: "assigned_hall");

            migrationBuilder.DropForeignKey(
                name: "FK_assigned_hall_students_S_ID",
                table: "assigned_hall");

            migrationBuilder.DropForeignKey(
                name: "FK_assigned_room_assigned_hall_S_ID_ASS_HALL_H_ID",
                table: "assigned_room");

            migrationBuilder.DropForeignKey(
                name: "FK_assigned_room_rooms_R_ID_ASS_HALL_H_ID",
                table: "assigned_room");

            migrationBuilder.DropForeignKey(
                name: "FK_assigned_room_students_S_ID",
                table: "assigned_room");

            migrationBuilder.DropForeignKey(
                name: "FK_hall_admins_hall_HALL_ID",
                table: "hall_admins");

            migrationBuilder.DropForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_rooms_ROOM_ID_HALL_ID",
                table: "MAINTENANCE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_students_STUDENT_ID",
                table: "MAINTENANCE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_room_application_requests_hall_HALL_ID",
                table: "room_application_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_room_application_requests_students_STUDENT_ID",
                table: "room_application_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_room_assignments_hall_HALL_ID",
                table: "room_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_room_assignments_students_STUDENT_ID",
                table: "room_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_rooms_REQUESTED_ROOM_ID_REQUESTED_HALL_~",
                table: "ROOM_CHANGE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_students_STUDENT_ID",
                table: "ROOM_CHANGE_REQUESTS");

            migrationBuilder.DropForeignKey(
                name: "FK_rooms_hall_H_ID",
                table: "rooms");

            migrationBuilder.DropTable(
                name: "boarder_registry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_students",
                table: "students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_room_assignments",
                table: "room_assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_room_application_requests",
                table: "room_application_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hall_admins",
                table: "hall_admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hall",
                table: "hall");

            migrationBuilder.DropPrimaryKey(
                name: "PK_assigned_room",
                table: "assigned_room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_assigned_hall",
                table: "assigned_hall");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admins",
                table: "admins");

            migrationBuilder.DropColumn(
                name: "S_BOARDER_NO",
                table: "students");

            migrationBuilder.DropColumn(
                name: "R_AVAILABLE",
                table: "rooms");

            migrationBuilder.RenameTable(
                name: "students",
                newName: "STUDENTS");

            migrationBuilder.RenameTable(
                name: "rooms",
                newName: "ROOMS");

            migrationBuilder.RenameTable(
                name: "room_assignments",
                newName: "ROOM_ASSIGNMENTS");

            migrationBuilder.RenameTable(
                name: "room_application_requests",
                newName: "ROOM_APPLICATION_REQUESTS");

            migrationBuilder.RenameTable(
                name: "hall_admins",
                newName: "HALL_ADMINS");

            migrationBuilder.RenameTable(
                name: "hall",
                newName: "HALL");

            migrationBuilder.RenameTable(
                name: "assigned_room",
                newName: "ASSIGNED_ROOM");

            migrationBuilder.RenameTable(
                name: "assigned_hall",
                newName: "ASSIGNED_HALL");

            migrationBuilder.RenameTable(
                name: "admins",
                newName: "ADMINS");

            migrationBuilder.RenameIndex(
                name: "IX_rooms_H_ID",
                table: "ROOMS",
                newName: "IX_ROOMS_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_room_assignments_STUDENT_ID",
                table: "ROOM_ASSIGNMENTS",
                newName: "IX_ROOM_ASSIGNMENTS_STUDENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_room_assignments_HALL_ID",
                table: "ROOM_ASSIGNMENTS",
                newName: "IX_ROOM_ASSIGNMENTS_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_room_application_requests_STUDENT_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                newName: "IX_ROOM_APPLICATION_REQUESTS_STUDENT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_room_application_requests_HALL_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                newName: "IX_ROOM_APPLICATION_REQUESTS_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_hall_admins_HALL_ID",
                table: "HALL_ADMINS",
                newName: "IX_HALL_ADMINS_HALL_ID");

            migrationBuilder.RenameIndex(
                name: "IX_assigned_room_S_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                newName: "IX_ASSIGNED_ROOM_S_ID_ASS_HALL_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_assigned_room_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                newName: "IX_ASSIGNED_ROOM_R_ID_ASS_HALL_H_ID");

            migrationBuilder.RenameIndex(
                name: "IX_assigned_hall_H_ID",
                table: "ASSIGNED_HALL",
                newName: "IX_ASSIGNED_HALL_H_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_STUDENTS",
                table: "STUDENTS",
                column: "S_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ROOMS",
                table: "ROOMS",
                columns: new[] { "R_ID", "H_ID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ROOM_ASSIGNMENTS",
                table: "ROOM_ASSIGNMENTS",
                column: "ASSIGNMENT_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ROOM_APPLICATION_REQUESTS",
                table: "ROOM_APPLICATION_REQUESTS",
                column: "APPLICATION_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HALL_ADMINS",
                table: "HALL_ADMINS",
                column: "HALL_ADMIN_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HALL",
                table: "HALL",
                column: "H_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ASSIGNED_ROOM",
                table: "ASSIGNED_ROOM",
                column: "ASS_ROOM_SID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ASSIGNED_HALL",
                table: "ASSIGNED_HALL",
                columns: new[] { "S_ID", "H_ID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ADMINS",
                table: "ADMINS",
                column: "ADMIN_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_HALL_HALL_H_ID",
                table: "ASSIGNED_HALL",
                column: "H_ID",
                principalTable: "HALL",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_HALL_STUDENTS_S_ID",
                table: "ASSIGNED_HALL",
                column: "S_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_ROOM_ASSIGNED_HALL_S_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "S_ID", "ASS_HALL_H_ID" },
                principalTable: "ASSIGNED_HALL",
                principalColumns: new[] { "S_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "ASS_HALL_H_ID" },
                principalTable: "ROOMS",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_ROOM_STUDENTS_S_ID",
                table: "ASSIGNED_ROOM",
                column: "S_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HALL_ADMINS_HALL_HALL_ID",
                table: "HALL_ADMINS",
                column: "HALL_ID",
                principalTable: "HALL",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_ROOMS_ROOM_ID_HALL_ID",
                table: "MAINTENANCE_REQUESTS",
                columns: new[] { "ROOM_ID", "HALL_ID" },
                principalTable: "ROOMS",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MAINTENANCE_REQUESTS_STUDENTS_STUDENT_ID",
                table: "MAINTENANCE_REQUESTS",
                column: "STUDENT_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_APPLICATION_REQUESTS_HALL_HALL_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                column: "HALL_ID",
                principalTable: "HALL",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_APPLICATION_REQUESTS_STUDENTS_STUDENT_ID",
                table: "ROOM_APPLICATION_REQUESTS",
                column: "STUDENT_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_ASSIGNMENTS_HALL_HALL_ID",
                table: "ROOM_ASSIGNMENTS",
                column: "HALL_ID",
                principalTable: "HALL",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_ASSIGNMENTS_STUDENTS_STUDENT_ID",
                table: "ROOM_ASSIGNMENTS",
                column: "STUDENT_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_ROOMS_REQUESTED_ROOM_ID_REQUESTED_HALL_~",
                table: "ROOM_CHANGE_REQUESTS",
                columns: new[] { "REQUESTED_ROOM_ID", "REQUESTED_HALL_ID" },
                principalTable: "ROOMS",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOM_CHANGE_REQUESTS_STUDENTS_STUDENT_ID",
                table: "ROOM_CHANGE_REQUESTS",
                column: "STUDENT_ID",
                principalTable: "STUDENTS",
                principalColumn: "S_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ROOMS_HALL_H_ID",
                table: "ROOMS",
                column: "H_ID",
                principalTable: "HALL",
                principalColumn: "H_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
