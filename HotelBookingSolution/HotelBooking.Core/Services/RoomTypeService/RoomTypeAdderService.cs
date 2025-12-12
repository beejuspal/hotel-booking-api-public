using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
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
    public class RoomTypeAdderService:IRoomTypeAdderService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        public RoomTypeAdderService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }



        public async Task<ServiceResponse<CreateRoomTypeResponseDTO>>  CreateRoomTypeAsync(CreateRoomTypeDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<CreateRoomTypeResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid request data"

                       );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);

            // Duplicate check
            bool exists = await _roomTypeRepository.RoomTypeExistsAsync(request.TypeName,request.HotelId,0);
            if (exists)
            {
                return ServiceResponse<CreateRoomTypeResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Room type name already exists."

                      );
            }

            // Create entity
            var roomType = new RoomType
            {
                TypeName = request.TypeName,
                AccessibilityFeatures = request.AccessibilityFeatures,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                HotelId = request.HotelId
            };

            // Save using repository
            int newId = await _roomTypeRepository.AddRoomTypeAsync(roomType);

           
            return ServiceResponse<CreateRoomTypeResponseDTO>.Success(new CreateRoomTypeResponseDTO
            {
                RoomTypeId = newId,
               
            }, "Room type added successful");
        }

    }
}
