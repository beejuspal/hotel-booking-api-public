using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IRoomAmenitiesRepository
    {
        Task<List<RoomAmenity>> GetByRoomTypeAsync(int roomTypeId);
        Task BulkInsertAsync(IEnumerable<RoomAmenity> entities);
        Task BulkUpdateAsync(int roomTypeId, IEnumerable<RoomAmenity> newEntities);
        Task BulkDeltaAsync(int roomTypeId, IEnumerable<int> amenityIds);


    }
}
