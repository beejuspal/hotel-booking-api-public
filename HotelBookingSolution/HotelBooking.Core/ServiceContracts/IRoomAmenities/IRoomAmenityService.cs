using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomAmenitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IRoomAmenities
{
    public interface IRoomAmenityService
    {
        Task<ServiceResponse<RoomAmeniitiesDto>> GetRoomAmenitiesAsync(int roomTypeId);
        Task BulkInsertAsync(RoomAmenityBulkDto dto);
        Task BulkUpdateAsync(RoomAmenityBulkDto dto);
        Task BulkDeltaAsync(RoomAmenityBulkDto dto);
    }

}
