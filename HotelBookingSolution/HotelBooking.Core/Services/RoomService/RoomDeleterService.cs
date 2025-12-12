using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IRoomService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.RoomService
{
    public class RoomDeleterService : IRoomDeleterService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomDeleterService(IRoomTypeRepository roomTypeRepository, IRoomRepository roomRepository, IReservationRepository reservationRepository)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
        }
        public async Task<ServiceResponse<object>> DeleteRoomAsync(int roomId)
        {
            
            var activeReserevationExists = await _reservationRepository.ActiveReservationExistsAsync(roomId);
            if(activeReserevationExists)
            {
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                             "Room cannot be deactivated, there are active reservations."
                         );

            }
            var room = await _roomRepository.RetrieveRoomByIdAsync(roomId);

            if (room == null)
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                             "Room not found"

                         );
            //if (await _roomRepository.RoomExistsByRoomTypeAsync(roomType.RoomTypeID))
            //{
            //    return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
            //                "Cannot delete room type as it is being referenced by one or more rooms."

            //            );
            //}

            await _roomRepository.DeleteRoomAsync(room);
            return ServiceResponse<object>.Success(null, "Room deleted successful");
        }
    }
}
