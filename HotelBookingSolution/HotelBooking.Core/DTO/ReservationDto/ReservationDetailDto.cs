using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.ReservationDto
{
    //public class ReservationDetailsDto
    //{
    //    public int ReservationID { get; set; }
    //    public DateTime BookingDate { get; set; }
    //    public DateTime CheckInDate { get; set; }
    //    public DateTime CheckOutDate { get; set; }
    //    public string Status { get; set; }
    //    public decimal TotalCost { get; set; }
    //    public int NumberOfNights { get; set; }

    //    public string UserName { get; set; }
    //    public List<ReservationRoomDto> Rooms { get; set; }
    //}

    public class ReservationDetailsDto
    {
        public int ReservationID { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalCost { get; set; }
        public string UserName { get; set; }
        public HotelDto Hotel { get; set; }
        public List<RoomDetailDto> Rooms { get; set; }
        public List<GuestDto> Guests { get; set; }
    }

    public class HotelDto
    {
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public string HotelLocation { get; set; }
        public int StarRating { get; set; }
    }

    public class RoomDetailDto
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public string BedType { get; set; }
        public string ViewType { get; set; }
        public string RoomTypeName { get; set; }
    }

    public class GuestDto
    {
        public int GuestID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AgeGroup { get; set; }
    }
}
