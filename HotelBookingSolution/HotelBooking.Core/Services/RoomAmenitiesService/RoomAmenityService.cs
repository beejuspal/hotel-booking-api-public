using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomAmenitites;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.ServiceContracts.IRoomAmenities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.RoomAmenitiesService
{
    public class RoomAmenityService : IRoomAmenityService
    {
        private readonly IRoomAmenitiesRepository _repository;

        public RoomAmenityService(IRoomAmenitiesRepository repository)
        {
            _repository = repository;
        }
        public async Task<ServiceResponse<RoomAmeniitiesDto>> GetRoomAmenitiesAsync(int roomTypeId)
        {
            var list = await _repository.GetByRoomTypeAsync(roomTypeId);


            if (list == null || list.Count == 0)
                return ServiceResponse<RoomAmeniitiesDto>.Fail(HttpStatusCode.NotFound,
                                "No any room amenities found"

                            );

            var result = list.Select(rt => new RoomAmeniitiesDto
            {

                RoomTypeID = rt.RoomTypeID,
                AmenityIDs = list.Select(x => x.AmenityID).ToList()
            }).FirstOrDefault();
            return ServiceResponse <RoomAmeniitiesDto>.Success(result, "Room retrieve successful");
        }

       

        public async Task BulkInsertAsync(RoomAmenityBulkDto dto)
        {
            var newEntities = dto.AmenityIDs.Select(aid => new RoomAmenity
            {
                RoomTypeID = dto.RoomTypeID,
                AmenityID = aid
            });
            await _repository.BulkInsertAsync(newEntities);
            //return ServiceResponse<RoomAmeniitiesDto>.Success(result, "Room retrieve successful");
        }

        public async Task BulkUpdateAsync(RoomAmenityBulkDto dto)
        {
            var newEntities = dto.AmenityIDs.Select(aid => new RoomAmenity
            {
                RoomTypeID = dto.RoomTypeID,
                AmenityID = aid
            });
            await _repository.BulkUpdateAsync(dto.RoomTypeID, newEntities);
        }

        public async Task BulkDeltaAsync(RoomAmenityBulkDto dto)
        {
            await _repository.BulkDeltaAsync(dto.RoomTypeID, dto.AmenityIDs);
        }
    }

}
