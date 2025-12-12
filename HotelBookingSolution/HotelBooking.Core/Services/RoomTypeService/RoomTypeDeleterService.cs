using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.RoomTypeService
{
    public class RoomTypeDeleterService : IRoomTypeDeleterService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IRoomRepository _roomRepository;


        public RoomTypeDeleterService(IRoomTypeRepository roomTypeRepository, IRoomRepository roomRepository)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
        }
        public async Task<ServiceResponse<object>> DeleteRoomTypeAsync(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.RetrieveRoomTypeByIdAsync(roomTypeId);

            if (roomType == null)
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                             "Room type not found"

                         );
            if (await _roomRepository.RoomExistsByRoomTypeAsync(roomType.RoomTypeID))
            {
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                            "Cannot delete room type as it is being referenced by one or more rooms."

                        );
            }

            await _roomTypeRepository.DeleteRoomTypeAsync(roomType);
            return ServiceResponse<object>.Success(null, "Room type deleted successful");
        }
    }
}
