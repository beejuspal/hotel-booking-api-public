using CloudinaryDotNet.Actions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.DashboardDto;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly HotelDbContext _context;

        private IDbContextTransaction? _transaction;
        public ReservationRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ActiveReservationExistsAsync(int roomId)
        {
            //var excludedStatuses = new[] { "Checked-out", "Cancelled" };
            //return await _context.Reservations.AnyAsync(rt => rt.RoomID == roomId && !excludedStatuses.Contains(rt.Status));
            return false;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task<int> AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation.ReservationID;
        }

        public async Task AddReservationRoomsAsync(List<ReservationRoom> reservationRooms)
        {
            _context.ReservationRooms.AddRange(reservationRooms);
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddGuestAsync(Guest guest)
        {
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
            return guest.GuestID;
        }
        public async Task AddReservationGuestAsync(List<RoomReservationGuest> reservationGuests)
        {
            _context.RoomReservationGuests.AddRange(reservationGuests);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ReservationRoom>> GetReservationRoomsByReservationIdAsync(int reservationId)
        {
            return await _context.ReservationRooms
                .Where(r => r.ReservationID == reservationId)
                .ToListAsync();
        }
        public async Task<(List<Reservation> Reservations, int TotalCount)> GetAdminFilteredReservationsAsync(ReservationAdminFilterDto filter, string role, int? HotelId)
        {
            var query = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.Room)
                        .ThenInclude(room => room.RoomType)
                         .ThenInclude(rt => rt.Hotel)
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.RoomReservationGuests)
                        .ThenInclude(rg => rg.Guest)

                .AsQueryable();
            // 🧩 Restrict by hotel if user is a HotelAdmin
            if (role == "Manager" && HotelId.HasValue)
            {
                query = query.Where(r =>
                    r.ReservationRooms.Any(rr => rr.Room.RoomType.HotelId == HotelId.Value));
            }
            // 🔍 Apply Search Filter
            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                string search = filter.SearchQuery.ToLower();

                query = query.Where(r =>
                    r.User.FullName.ToLower().Contains(search) ||
                    r.ReservationRooms.Any(rr =>
                        rr.Room.RoomType.Hotel.Name.ToLower().Contains(search)) ||
                    r.ReservationID.ToString().Contains(search)
                );
            }
            // 🔹 Apply filters
            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.Status == filter.Status);

            // Apply date filter (ignore time)
            if (filter.StartDate.HasValue)
                query = query.Where(r => r.CheckInDate.Date >= filter.StartDate.Value.Date);

            if (filter.EndDate.HasValue)
                query = query.Where(r => r.CheckOutDate.Date <= filter.EndDate.Value.Date);

            var totalCount = await query.CountAsync();

            // 🔹 Pagination
            var reservations = await query
                .OrderByDescending(r => r.BookingDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (reservations, totalCount);
        }

        public async Task<(List<Reservation> Reservations, int TotalCount)> GetFilteredReservationsByUserAsync(int userId, ReservationFilterDto filter)
        {
            var query = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.Room)
                        .ThenInclude(room => room.RoomType)
                         .ThenInclude(rt => rt.Hotel)
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.RoomReservationGuests)
                        .ThenInclude(rg => rg.Guest)
                .Where(r => r.UserID == userId)
                .AsQueryable();

            // 🔹 Apply filters
            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(r => r.Status == filter.Status);

            // Apply date filter (ignore time)
            if (filter.StartDate.HasValue)
                query = query.Where(r => r.CheckInDate.Date >= filter.StartDate.Value.Date);

            if (filter.EndDate.HasValue)
                query = query.Where(r => r.CheckOutDate.Date <= filter.EndDate.Value.Date);

            var totalCount = await query.CountAsync();

            // 🔹 Pagination
            var reservations = await query
                .OrderByDescending(r => r.BookingDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (reservations, totalCount);
        }

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
        {
            var today = DateTime.UtcNow.Date; // midnight UTC

            return await _context.Reservations
                .Include(r => r.ReservationRooms)
                .Where(r => r.CheckOutDate < today
                            && (r.Status == "Reserved" || r.Status == "Completed"))
                .ToListAsync();

        }

        public async Task<Reservation> GetReservationsByIdAsync(int reservationId)
        {
            return await _context.Reservations.Include(x => x.User).Include(x => x.ReservationRooms).ThenInclude(x => x.Room).ThenInclude(x => x.RoomType).Where(x => x.ReservationID == reservationId).FirstOrDefaultAsync();
        }

        public void UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }
        public async Task AddReservationCancellAsync(Cancellation cancellation)
        {
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();

        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetAllReservationsByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Where(r => r.UserID == userId)
                .CountAsync();
        }
        public async Task<List<Reservation>> GetLasttSixMothReservationAsync(int userId)
        {
            return await _context.Reservations
        .Where(r => r.UserID == userId && r.CreatedDate > DateTime.UtcNow.AddMonths(-6))
        .ToListAsync();
        }
    }
}
