using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_sp_spSearchHotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"
       CREATE OR ALTER PROCEDURE SearchHotelRoomAmenities
    @MinPrice DECIMAL(10,2) = NULL,
    @MaxPrice DECIMAL(10,2) = NULL,
    @RoomTypeName NVARCHAR(50) = NULL,
    @AmenityName NVARCHAR(100) = NULL,
    @ViewType NVARCHAR(50) = NULL,
    @HotelID INT = NULL
AS
BEGIN
    SET NOCOUNT ON; -- Suppress ""rows affected"" messages

    DECLARE @SQL NVARCHAR(MAX);

    SET @SQL = '
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
        h.HotelName,
        h.Location AS HotelLocation
    FROM Rooms r
    JOIN RoomTypes rt ON r.RoomTypeID = rt.RoomTypeID
    JOIN Hotels h ON r.HotelID = h.HotelID
    LEFT JOIN RoomAmenities ra ON rt.RoomTypeID = ra.RoomTypeID
    LEFT JOIN Amenities a ON ra.AmenityID = a.AmenityID
    WHERE r.IsActive = 1 ';

    DECLARE @Conditions NVARCHAR(MAX) = '';

    -- Dynamic filters
    IF @MinPrice IS NOT NULL
        SET @Conditions += ' AND r.Price >= @MinPrice ';
    
    IF @MaxPrice IS NOT NULL
        SET @Conditions += ' AND r.Price <= @MaxPrice ';
    
    IF @RoomTypeName IS NOT NULL
        SET @Conditions += ' AND rt.TypeName LIKE ''%' + @RoomTypeName + '%'' ';
    
    IF @AmenityName IS NOT NULL
        SET @Conditions += ' AND a.Name LIKE ''%' + @AmenityName + '%'' ';
    
    IF @ViewType IS NOT NULL
        SET @Conditions += ' AND r.ViewType = @ViewType ';
    
    IF @HotelID IS NOT NULL
        SET @Conditions += ' AND h.HotelID = @HotelID ';

    -- Add conditions if any exist
    IF LEN(@Conditions) > 0
        SET @SQL = @SQL + @Conditions;

    -- Execute dynamic SQL safely
    EXEC sp_executesql 
        @SQL,
        N'@MinPrice DECIMAL(10,2), 
          @MaxPrice DECIMAL(10,2), 
          @RoomTypeName NVARCHAR(50), 
          @AmenityName NVARCHAR(100), 
          @ViewType NVARCHAR(50),
          @HotelID INT',
        @MinPrice, @MaxPrice, @RoomTypeName, @AmenityName, @ViewType, @HotelID;
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
