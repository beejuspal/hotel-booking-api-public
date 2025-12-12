using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alter_sp_SearchHotelRoomAmenities_clean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"
       CREATE OR ALTER PROCEDURE [dbo].[SearchHotelRoomAmenities]
    @MinPrice DECIMAL(10,2) = NULL,
    @MaxPrice DECIMAL(10,2) = NULL,
    @RoomTypeName NVARCHAR(50) = NULL,
    @AmenityName NVARCHAR(100) = NULL,
    @ViewType NVARCHAR(50) = NULL,
    @HotelID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT 
        r.RoomID, 
        r.RoomNumber, 
        r.Price, 
        r.BedType, 
        r.ViewType, 
        r.Status, 
        rt.RoomTypeID, 
        rt.TypeName, 
        rt.AccessibilityFeatures, 
        rt.Description,
        h.HotelID,
        h.Name AS HotelName,
        h.Address AS HotelLocation
    FROM Rooms r
    JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
    JOIN Hotels h ON rt.HotelId = h.HotelId
    LEFT JOIN RoomAmenities ra ON rt.RoomTypeID = ra.RoomTypeID
    LEFT JOIN Amenities a ON ra.AmenityID = a.AmenityID
    WHERE 
        r.IsActive = 1
        AND (@MinPrice IS NULL OR r.Price >= @MinPrice)
        AND (@MaxPrice IS NULL OR r.Price <= @MaxPrice)
        AND (@RoomTypeName IS NULL OR rt.TypeName LIKE '%' + @RoomTypeName + '%')
        AND (@AmenityName IS NULL OR a.Name LIKE '%' + @AmenityName + '%')
        AND (@ViewType IS NULL OR r.ViewType = @ViewType)
        AND (@HotelID IS NULL OR h.HotelId = @HotelID);
END
";

            migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS SearchHotelRoomAmenities");
        }
    }
}
