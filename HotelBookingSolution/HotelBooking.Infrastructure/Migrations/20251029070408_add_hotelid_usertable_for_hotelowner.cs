using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_hotelid_usertable_for_hotelowner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelID",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HotelID",
                table: "Users",
                column: "HotelID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_HotelID",
                table: "Users",
                column: "HotelID",
                principalTable: "Hotels",
                principalColumn: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_HotelID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HotelID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HotelID",
                table: "Users");
        }
    }
}
