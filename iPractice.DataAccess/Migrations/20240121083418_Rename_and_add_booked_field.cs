using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iPractice.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Rename_and_add_booked_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "TimeSlots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "TimeSlots",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ClientId",
                table: "TimeSlots",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Clients_ClientId",
                table: "TimeSlots",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Clients_ClientId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_ClientId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "TimeSlots");
        }
    }
}
