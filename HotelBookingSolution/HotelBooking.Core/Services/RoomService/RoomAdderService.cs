using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
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
    public class RoomAdderService : IRoomAdderService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly Cloudinary _cloudinary;
        public RoomAdderService(IRoomTypeRepository roomTypeRepository, IRoomRepository roomRepository, Cloudinary cloudinary)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
            _cloudinary = cloudinary;
        }



        public async Task<ServiceResponse<CreateRoomResponseDTO>> CreateRoomAsync(CreateRoomRequestDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<CreateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid request data"

                       );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);

            var roomTypeExist = await _roomTypeRepository.RetrieveRoomTypeByIdAsync(request.RoomTypeID);
            if(roomTypeExist == null) return ServiceResponse<CreateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid Room Type ID provided."

                       );
            // Duplicate check
            bool exists = await _roomRepository.RoomExistsAsync(request.RoomNumber,request.RoomTypeID,0);
            if (exists)
            {
                return ServiceResponse<CreateRoomResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Room number already exists."

                      );
            }



            // Create entity
            var room = new Room
            {
              
                RoomNumber = request.RoomNumber,
                RoomTypeID = request.RoomTypeID,
                Price = request.Price,
                BedType = request.BedType,
                ViewType = request.ViewType,
                IsActive = request.IsActive,
                Status = request.Status,

                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
            };
            if (request.RoomImgs != null && request.RoomImgs.Any())
            {
                foreach (var file in request.RoomImgs.Take(5))
                {
                    if (file == null || file.Length == 0) continue;
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "rooms"
                    };
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    room.RoomImgs.Add(new RoomImage
                    {
                        ImageUrl = result.SecureUrl.AbsoluteUri
                    });
                }
            }
            // Save using repository
            int newId = await _roomRepository.AddRoomAsync(room);

           
            return ServiceResponse<CreateRoomResponseDTO>.Success(new CreateRoomResponseDTO
            {
                RoomID = newId,
               
            }, "Room added successful");
        }

    }
}
