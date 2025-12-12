using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.PaymentDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HotelDbContext _context;

        public PaymentRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

        }
        public async Task UpdatePaymentStatusAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
        public async Task<Payment> GetPaymentsByTransactionIdAsync(string transactionId)
        {

            return await _context.Payments.Where(x => x.TransactionId == transactionId).Include(x => x.Reservation).FirstOrDefaultAsync();
        }
        public async Task<(List<Payment> Payments, int TotalCount)> GetPaymentsAsync(string role, int userId, int? hotelId, PaymentFilterDto filter)
        {


            var query = _context.Payments
      .Include(p => p.Reservation)
          .ThenInclude(r => r.User)
      .Include(p => p.Reservation)
          .ThenInclude(r => r.ReservationRooms)
              .ThenInclude(rr => rr.Room)
                  .ThenInclude(room => room.RoomType)
                      .ThenInclude(rt => rt.Hotel).Include(p => p.Reservation).ThenInclude(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.RoomReservationGuests)
                        .ThenInclude(rg => rg.Guest).AsQueryable();

            if (role == "Manager" && hotelId.HasValue)
            {
                // Owner sees only payments for rooms in their hotel
                query = query.Where(p =>
                    p.Reservation.ReservationRooms
                        .Any(rr => rr.Room.RoomType.HotelId == hotelId.Value));
            }
            else if (role == "Guest")
            {
                // User sees only their own reservation payments
                query = query.Where(p => p.Reservation.UserID == userId);
            }
            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                string search = filter.SearchQuery.ToLower();

                query = query.Where(r =>
                    r.Reservation.User.FullName.ToLower().Contains(search) ||
                    r.Reservation.ReservationRooms.Any(rr =>
                        rr.Room.RoomType.Hotel.Name.ToLower().Contains(search)) ||
                    r.ReservationID.ToString().Contains(search)
                );
            }
            // 🔹 Apply filters
            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.PaymentStatus == filter.Status);
            var totalCount = await query.CountAsync();

            // 🔹 Pagination
            var payments = await query
                .OrderByDescending(r => r.PaymentDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (payments, totalCount);
        }

        public async Task<List<Payment>> GePaymentByStatus(int userId,string status)
        {
            var query = _context.Payments
     .Include(p => p.Reservation)
         .ThenInclude(r => r.User)
     .Include(p => p.Reservation)
         .ThenInclude(r => r.ReservationRooms)
             .ThenInclude(rr => rr.Room)
                 .ThenInclude(room => room.RoomType)
                     .ThenInclude(rt => rt.Hotel).Include(p => p.Reservation).ThenInclude(r => r.ReservationRooms)
                   .ThenInclude(rr => rr.RoomReservationGuests)
                       .ThenInclude(rg => rg.Guest).AsQueryable();
            if (string.IsNullOrEmpty(status))
            {
                return await query
                .Where(p => p.Reservation.UserID == userId).ToListAsync();
            }
           
            return await query
                .Where(p => p.Reservation.UserID == userId && p.PaymentStatus == status).ToListAsync();
                
        }

    }
}
