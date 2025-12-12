using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.DashboardDto;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.ServiceContracts.IDashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.Dashbaord
{
    public class DashboardService : IDashboardService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPaymentRepository _paymentRepository;

        public DashboardService(IReservationRepository reservationRepository, IPaymentRepository paymentRepository)
        {
            _reservationRepository = reservationRepository;
            _paymentRepository = paymentRepository;


        }
        public async Task<ServiceResponse<UserDashboardDto>>  GetUserDashboardAsync(int userId)
        {
            var dto = new UserDashboardDto();

            // Total Reservations
            var resCount = await _reservationRepository.GetAllReservationsByUserIdAsync(userId);
            dto.TotalReservations = resCount;

            // Total Payments
            var completePaymentCount = await _paymentRepository.GePaymentByStatus(userId, "Completed");

            dto.TotalPayments = completePaymentCount.Count();

            // Pending Payments

            var pendignPaymentCount = await _paymentRepository.GePaymentByStatus(userId, "Pending");

            dto.PendingPayments = pendignPaymentCount.Count();


            // Recent Payments (last 5)
            var recentPay = await _paymentRepository.GePaymentByStatus(userId, "");
            dto.RecentPayments = recentPay
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .Select(p => new RecentPaymentDto
                {
                    ReservationID = p.ReservationID,
                    HotelName = p.Reservation.ReservationRooms
                        .Select(rr => rr.Room.RoomType.Hotel.Name)
                        .FirstOrDefault(),
                    PaymentStatus = p.PaymentStatus,
                    TotalAmount = p.TotalAmount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod=p.PaymentMethod
                })
                .ToList();

            // Monthly Reservations (last 6 months)

            var reservations = await _reservationRepository.GetLasttSixMothReservationAsync(userId);

            var monthlyData = reservations
    .GroupBy(r => new { r.CreatedDate.Month, r.CreatedDate.Year })
    .Select(g => new MonthlyReservationDto
    {
        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month),
        Count = g.Count()
    })
    .OrderBy(x => x.Month)
    .ToList();

            dto.MonthlyReservations = monthlyData;
            // Payments by Status (for Pie Chart)

            dto.PaymentStatusCounts = recentPay
                .GroupBy(p => p.PaymentStatus)
                .Select(g => new PaymentStatusCountDto
                {
                    Status = g.Key!,
                    Count = g.Count()
                })
                .ToList();

         
            return ServiceResponse<UserDashboardDto>.Success(dto, "Reservations retrieve successful");
        }
    }
}
