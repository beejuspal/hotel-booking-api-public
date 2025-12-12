using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.ReservationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IReservationRepository
    {
        //-- Ensure no active reservations exist for the room
        //IF NOT EXISTS(SELECT 1 FROM Reservations WHERE RoomID = @RoomID AND Status NOT IN ('Checked-out', 'Cancelled'))
        Task<bool> ActiveReservationExistsAsync(int roomId);
        Task<int> AddReservationAsync(Reservation reservation);
        Task AddReservationRoomsAsync(List<ReservationRoom> reservationRooms);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
       
        Task<int> AddGuestAsync(Guest guest);
        Task AddReservationGuestAsync(List<RoomReservationGuest> reservationGuests);
        Task<List<ReservationRoom>> GetReservationRoomsByReservationIdAsync(int reservationId);
        Task<(List<Reservation> Reservations, int TotalCount)> GetAdminFilteredReservationsAsync(ReservationAdminFilterDto filter, string role, int? HotelId);
        Task<(List<Reservation> Reservations, int TotalCount)> GetFilteredReservationsByUserAsync(int userId, ReservationFilterDto filter);
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
        void UpdateReservationAsync(Reservation reservation);
        Task<Reservation> GetReservationsByIdAsync(int reservationId);
        Task AddReservationCancellAsync(Cancellation cancellation);
        Task SaveChangesAsync();
        Task<List<Reservation>> GetLasttSixMothReservationAsync(int userId);
        Task<int> GetAllReservationsByUserIdAsync(int userId);
    }
}
