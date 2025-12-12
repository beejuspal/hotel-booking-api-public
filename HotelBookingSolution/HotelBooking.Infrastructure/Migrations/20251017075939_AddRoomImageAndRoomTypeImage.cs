using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomImageAndRoomTypeImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add RoomTypeImage column only if it doesn't exist
            migrationBuilder.Sql(@"
        IF NOT EXISTS(
            SELECT * 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'RoomTypes' 
              AND COLUMN_NAME = 'RoomTypeImage')
        BEGIN
            ALTER TABLE RoomTypes ADD RoomTypeImage NVARCHAR(MAX) NULL;
        END
    ");

            // Create RoomImages table if it doesn't exist
            migrationBuilder.Sql(@"
        IF NOT EXISTS(
            SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'RoomImages')
        BEGIN
            CREATE TABLE RoomImages (
                RoomImageId INT IDENTITY(1,1) PRIMARY KEY,
                ImageUrl NVARCHAR(MAX) NOT NULL,
                Caption NVARCHAR(MAX) NOT NULL,
                RoomID INT NOT NULL,
                CONSTRAINT FK_RoomImages_Rooms FOREIGN KEY (RoomID)
                    REFERENCES Rooms(RoomId) ON DELETE CASCADE
            );
        END
    ");
        }

    }
}
