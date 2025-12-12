using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IHotelService;
using HotelBooking.Core.ServiceContracts.IRoomService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.RoomService
{
    public class RoomUpdaterService : IRoomUpdaterService
    {
        private readonly IImageStorageService _imageStorage;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IRoomRepository _roomRepository;
        public RoomUpdaterService(IRoomTypeRepository roomTypeRepository, IRoomRepository roomRepository, IImageStorageService imageStorage)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
            _imageStorage = imageStorage;
        }
        public async Task<ServiceResponse<UpdateRoomResponseDTO>> UpdateRoomAsync(UpdateRoomRequestDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<UpdateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                            "Invalid request"

                        );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);


            var roomTypeExist = await _roomTypeRepository.RetrieveRoomTypeByIdAsync(request.RoomTypeID);
            if (roomTypeExist == null) return ServiceResponse<UpdateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid Room Type ID provided."

                       );
            var rm = await _roomRepository.RetrieveRoomByIdAsync(request.RoomID);
            if (rm == null)
            {
                return ServiceResponse<UpdateRoomResponseDTO>.Fail(HttpStatusCode.NotFound,
                            "Room not found"

                        );
            }
            // Duplicate check
            bool exists = await _roomRepository.RoomExistsAsync(request.RoomNumber, request.RoomTypeID, request.RoomID);
            if (exists)
            {
                return ServiceResponse<UpdateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Room number already exists."

                      );
            }

            // update entity
        

            rm.RoomNumber = request.RoomNumber;
            rm.Price = request.Price;
            rm.BedType = request.BedType;
            rm.ModifiedDate = DateTime.UtcNow;
            rm.ModifiedBy = request.ModifiedBy;
            rm.RoomTypeID = request.RoomTypeID;
           rm.Status = request.Status;
            rm.ViewType = request.ViewType;
            rm.IsActive=request.IsActive;
            await HandleImagesAsync(rm, request);

            int updateRoomId=await _roomRepository.UpdateRoomAsync(rm);
            
            return ServiceResponse<UpdateRoomResponseDTO>.Success(new UpdateRoomResponseDTO
            {
                RoomId = updateRoomId,
              
            }, "Room updated successful");
        }

        private async Task HandleImagesAsync(Room room, UpdateRoomRequestDTO dto)
        {
            var existingUrls = dto.ExistingImgs ?? new List<string>();

            // Delete removed images
            foreach (var delImg in room.RoomImgs.Where(img => !existingUrls.Contains(img.ImageUrl)).ToList())
            {
                await _imageStorage.DeleteAsync(delImg.ImageUrl);
                await _roomRepository.DeleteRoomImageAsync(delImg);
            }

            // Upload new images
            if (dto.RoomImgs != null)
            {
                foreach (var file in dto.RoomImgs.Take(5))
                {
                    var uploadedUrl = await _imageStorage.UploadAsync(file, "hotels");
                    room.RoomImgs.Add(new RoomImage { ImageUrl = uploadedUrl });
                }
            }
        }
    }
}
