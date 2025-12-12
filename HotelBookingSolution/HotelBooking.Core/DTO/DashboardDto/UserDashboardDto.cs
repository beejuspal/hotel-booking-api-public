using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.DashboardDto
{
    // DTOs/Dashboard/UserDashboardDto.cs
    public class UserDashboardDto
    {
        public int TotalReservations { get; set; }
        public int TotalPayments { get; set; }
        public int PendingPayments { get; set; }
        public List<RecentPaymentDto> RecentPayments { get; set; } = new();
        public List<MonthlyReservationDto> MonthlyReservations { get; set; } = new();
        public List<PaymentStatusCountDto> PaymentStatusCounts { get; set; } = new();
    }

    public class RecentPaymentDto
    {
        public int ReservationID { get; set; }
        public string? HotelName { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class MonthlyReservationDto
    {
        public string Month { get; set; } = "";
        public int Count { get; set; }
    }

    public class PaymentStatusCountDto
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

}
