using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.HotelDto
{
    public  class FeaturedHotelDto
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string HotelLocation { get; set; }
        public string Description { get; set; }
        public int StarRating { get; set; }
        public List<FeaturedHotelImageDto> HotelImages { get; set; }
        public List<FeaturedRoomImageDto> RoomImages { get; set; }
        public List<FeaturedRoomTypeDto> RoomTypes { get; set; }
       

        //public class HotelImageDto
        //{
        //    public int HotelImageID { get; set; }
        //    public string ImageUrl { get; set; }
        //}

        //public class HotelsDto
        //{
        //    public int HotelID { get; set; }
        //    public string HotelName { get; set; }
        //    public string HotelLocation { get; set; }
        //    public string Description { get; set; }
        //    public int StarRating { get; set; }
        //    public List<HotelImageDto> HotelImages { get; set; }
        //    public List<RoomTypeDto> RoomTypes { get; set; }
        //}
    }
    public class FeaturedHotelImageDto
    {
        public int HotelImageID { get; set; }
        public string ImageUrl { get; set; }
    }
    public class FeaturedRoomImageDto
    {
        public int RoomImageID { get; set; }
        public string ImageUrl { get; set; }
    }
    public class FeaturedAmenityDto
    {
        public int AmenityID { get; set; }
        public string Name { get; set; }
    }

    public class FeaturedRoomDto
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public string BedType { get; set; }
        public string ViewType { get; set; }
        public string Status { get; set; }
        public List<FeaturedRoomImageDto> RoomImages { get; set; }
    }

    public class FeaturedRoomTypeDto
    {
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; }
        public string AccessibilityFeatures { get; set; }
        public string Description { get; set; }
        public List<FeaturedRoomDto> Rooms { get; set; }
        public List<FeaturedAmenityDto> Amenities { get; set; }
    }
}
