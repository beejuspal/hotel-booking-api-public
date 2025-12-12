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
    public class RoomTypeUpdaterService : IRoomTypeUpdaterService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeUpdaterService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }
        public async Task<ServiceResponse<UpdateRoomTypeResponseDTO>>  UpdateRoomTypeAsync(UpdateRoomTypeDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<UpdateRoomTypeResponseDTO>.Fail(HttpStatusCode.BadRequest,
                            "Invalid request"

                        );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);

          
            var rmType = await _roomTypeRepository.RetrieveRoomTypeByIdAsync(request.RoomTypeID);
            if (rmType == null)
            {
                return ServiceResponse<UpdateRoomTypeResponseDTO>.Fail(HttpStatusCode.NotFound,
                            "Room type not found"

                        );
            }
            // Duplicate check
            bool exists = await _roomTypeRepository.RoomTypeExistsAsync(request.TypeName, request.HotelId, rmType.RoomTypeID);
            if (exists)
            {
                return ServiceResponse<UpdateRoomTypeResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Room type name already exists."

                      );
            }
           
            // update entity
            rmType.TypeName = request.TypeName;
            rmType.AccessibilityFeatures = request.AccessibilityFeatures;
            rmType.Description = request.Description;
            rmType.ModifiedDate = DateTime.UtcNow;
            rmType.ModifiedBy = request.ModifiedBy;
            rmType.HotelId = request.HotelId;
            rmType.IsActive = request.IsActive;
           
           
            int updateRoomTypeId=await _roomTypeRepository.UpdateRoomTypeAsync(rmType);
            
            return ServiceResponse<UpdateRoomTypeResponseDTO>.Success(new UpdateRoomTypeResponseDTO
            {
                RoomTypeId = updateRoomTypeId,
              
            }, "Room type updated successful");
        }
    }
}
