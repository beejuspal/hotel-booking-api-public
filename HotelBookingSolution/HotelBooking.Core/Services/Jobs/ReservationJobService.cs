using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.ReservationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.Jobs
{
    public class ReservationJobService
    {
       
        private readonly IReservationRepository _repo;
        private readonly IRoomRepository _roomRepository;
        public ReservationJobService( IReservationRepository repo, IRoomRepository roomRepository)
        {
           
            _repo = repo;
            _roomRepository = roomRepository;
        }

        public async Task UpdateExpiredReservationsAsync()
        {
            // ✅ Begin a transaction manually
            await _repo.BeginTransactionAsync();

            try
            {
                var expiredReservations = await _repo.GetExpiredReservationsAsync();

                foreach (var res in expiredReservations)
                {
                    res.ModifiedDate = DateTime.UtcNow;
                    res.Status =res.Status=="Completed"?"Completed": "Expired";
                    _repo.UpdateReservationAsync(res);
                }
                if (expiredReservations!=null && expiredReservations.Count() > 0)
                {
                    var roomIds = expiredReservations.SelectMany(r => r.ReservationRooms.Select(rm => rm.RoomID)).ToList();
                    await _roomRepository.UpdateRoomStatusesAsync(roomIds, "Available"); // Track updates only
                }
                // Update related rooms (assuming all in one DB transaction)
               
                await _repo.SaveChangesAsync();

                await _repo.CommitTransactionAsync(); // ✅ Commit if everything succeeded

                Console.WriteLine($"✅ {expiredReservations.Count()} reservations marked as Completed at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                await _repo.RollbackTransactionAsync(); // ❌ Rollback everything if one fails
                Console.WriteLine($"❌ Error updating reservations: {ex.Message}");
            }
        }
    }
}
