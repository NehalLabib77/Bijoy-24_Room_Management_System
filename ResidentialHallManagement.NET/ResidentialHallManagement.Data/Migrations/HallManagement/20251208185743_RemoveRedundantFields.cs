using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResidentialHallManagement.Data.Migrations.HallManagement
{
    /// <inheritdoc />
    public partial class RemoveRedundantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_R_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_R_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropColumn(
                name: "R_AVAILABILITY",
                table: "ROOMS");

            migrationBuilder.DropColumn(
                name: "R_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "ASS_HALL_H_ID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "ASS_HALL_H_ID" },
                principalTable: "ROOMS",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.DropIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_ASS_HALL_H_ID",
                table: "ASSIGNED_ROOM");

            migrationBuilder.AddColumn<string>(
                name: "R_AVAILABILITY",
                table: "ROOMS",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "R_H_ID",
                table: "ASSIGNED_ROOM",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNED_ROOM_R_ID_R_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "R_H_ID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ASSIGNED_ROOM_ROOMS_R_ID_R_H_ID",
                table: "ASSIGNED_ROOM",
                columns: new[] { "R_ID", "R_H_ID" },
                principalTable: "ROOMS",
                principalColumns: new[] { "R_ID", "H_ID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
