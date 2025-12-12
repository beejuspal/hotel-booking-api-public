using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomCostDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IRoomService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class RoomGetterService : IRoomGetterService
    {
        private readonly IRoomRepository _roomRepository;
        private const decimal GST_RATE = 0.13m;
        public RoomGetterService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }



        public async Task<ServiceResponse<List<RoomDto>>> RetrieveAllRoomsAsync()
        {
            var roomList = await _roomRepository.RetrieveAllRoomsAsync();


            if (roomList == null || roomList.Count == 0)
                return ServiceResponse<List<RoomDto>>.Fail(HttpStatusCode.NotFound,
                                "No any room found"

                            );
            //rm.RoomNumber = request.RoomNumber;
            //rm.Price = request.Price;
            //rm.BedType = request.BedType;
            //rm.ModifiedDate = DateTime.UtcNow;
            //rm.ModifiedBy = request.ModifiedBy;
            //rm.RoomTypeID = request.RoomTypeID;
            //rm.Status = request.Status;
            //rm.ViewType = request.ViewType;
            //rm.IsActive = request.IsActive;
            // Map entity list to DTO list
            var result = roomList.Select(rt => new RoomDto
            {
                RoomID = rt.RoomID,
                RoomTypeID = rt.RoomTypeID,
                RoomNumber = rt.RoomNumber,
                IsActive = rt.IsActive,
                Price = rt.Price,
                BedType = rt.BedType,
                ViewType = rt.ViewType,
                Status = rt.Status
            }).ToList();
            return ServiceResponse<List<RoomDto>>.Success(result, "Room retrieve successful");
        }

        public async Task<ServiceResponse<RoomDto>> RetrieveRoomByIdAsync(int roomId)
        {
            var room = await _roomRepository.RetrieveRoomByIdAsync(roomId);

            if (room == null)
                return ServiceResponse<RoomDto>.Fail(HttpStatusCode.NotFound,
                                 "Room not found"

                             );

            var res = new RoomDto
            {
                RoomID = room.RoomID,
                RoomTypeID = room.RoomTypeID,
                RoomNumber = room.RoomNumber,
                IsActive = room.IsActive,
                Price = room.Price,
                BedType = room.BedType,
                ViewType = room.ViewType,
                Status = room.Status,
                HotelId = room.RoomType.HotelId ,RoomImageUrls = room.RoomImgs.Select(img => new RoomImageDto
                {
                    Id = img.RoomImageId,
                    Url = img.ImageUrl
                }).ToList()
            };
            return ServiceResponse<RoomDto>.Success(res, "Room found");
        }

        public async Task<ServiceResponse<PagedDataResultDto<RoomDto>>> RetrieveAllRoomsWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            var roomList = await _roomRepository.RetrieveAllRoomsWithPaginationAsync(pageNumber, pageSize);


            if (roomList.Item1 == null || roomList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<RoomDto>>.Fail(HttpStatusCode.NotFound,
                                "No any room detail found"

                            );


            // Map entity list to DTO list
            var result = roomList.Item1.Select(room => new RoomDto
            {
                RoomID = room.RoomID,
                RoomTypeID = room.RoomTypeID,
                RoomNumber = room.RoomNumber,
                IsActive = room.IsActive,
                Price = room.Price,
                BedType = room.BedType,
                ViewType = room.ViewType,
                Status = room.Status,
                RoomImageUrls = room.RoomImgs.Select(img => new RoomImageDto
                {
                    Id = img.RoomImageId,
                    Url = img.ImageUrl
                }).ToList()
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<RoomDto>
            {
                Items = result,
                TotalRecords = roomList.totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(roomList.totalRecords / (double)pageSize)
            };
            return ServiceResponse<PagedDataResultDto<RoomDto>>.Success(pagedDtoResult, "Room retrieve successful");
        }

        public async Task<ServiceResponse<List<RoomDto>>> RetrieveRoomsByIdsAsync(List<int> roomIds)
        {
            var rooms = await _roomRepository.RetrieveRoomsByIdsAsync(roomIds);

            if (rooms == null || rooms.Count <= 0)
                return ServiceResponse<List<RoomDto>>.Fail(HttpStatusCode.NotFound,
                                 "Room not found"

                             );

            //var res = new RoomDto
            //{
            //    RoomID = room.RoomID,
            //    RoomTypeID = room.RoomTypeID,
            //    RoomNumber = room.RoomNumber,
            //    IsActive = room.IsActive,
            //    Price = room.Price,
            //    BedType = room.BedType,
            //    ViewType = room.ViewType,
            //    Status = room.Status,
            //    HotelId = room.RoomType.HotelId
            //};
            var roomDetails = rooms.Select(room => new RoomDto
            {
                RoomID = room.RoomID,
                RoomTypeID = room.RoomTypeID,
                RoomNumber = room.RoomNumber,
                IsActive = room.IsActive,
                Price = room.Price,
                BedType = room.BedType,
                ViewType = room.ViewType,
                Status = room.Status,
                HotelId = room.RoomType.HotelId
            }).ToList();
            return ServiceResponse<List<RoomDto>>.Success(roomDetails, "Room found");
        }

        public async Task<ServiceResponse<RoomCostResultDto>> CalculateRoomCostsAsync(List<int> roomIds, DateTime checkInDate, DateTime checkOutDate)
        {
            // 🧩 Validation: Missing or invalid data
            if (roomIds == null || !roomIds.Any())
            {
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                            "At least one room must be selected."

                         );
            }
                
           

            if (checkInDate == default || checkOutDate == default)
            {
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                           "Check-in and check-out dates are required."

                        );
            }
               

            if (checkOutDate <= checkInDate)
            {
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                           "Check-out date must be later than check-in date."

                        );

            }
                

            int numberOfNights = (checkOutDate - checkInDate).Days;

            var rooms = await _roomRepository.RetrieveRoomsByIdsAsync(roomIds);

            if (rooms == null || rooms.Count == 0)
            {
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                             "No valid rooms found for the provided IDs."

                          );
            }
            if (rooms.Count != roomIds.Count)
            {
                var missingIds = roomIds.Except(rooms.Select(r => r.RoomID)).ToList();
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                             $"Rooms not found for IDs: {string.Join(", ", missingIds)}"

                          );
            }
            // ✅ Check if any room is occupied
            var occupiedRoom = rooms.FirstOrDefault(r => r.Status.Equals("Occupied", StringComparison.OrdinalIgnoreCase));

            if (occupiedRoom != null)
            {
                return ServiceResponse<RoomCostResultDto>.Fail(HttpStatusCode.NotFound,
                          $"Room '{occupiedRoom.RoomNumber}' is currently occupied and cannot be booked."

                         );
            }


            var roomDetails = rooms.Select(r => new RoomCostDetailDto
            {
                RoomID = r.RoomID,
                RoomNumber = r.RoomNumber,
                RoomPrice = r.Price,
                NumberOfNights = numberOfNights,
                TotalPrice = r.Price * numberOfNights,
                RoomStatus = r.Status,RoomTypeID=r.RoomTypeID
            }).ToList();

            decimal baseAmount = roomDetails.Sum(r => r.TotalPrice);
            decimal gst = Math.Round(baseAmount * GST_RATE, 2);
            decimal totalAmount = baseAmount + gst;

            var res = new RoomCostResultDto
            {
                CheckIn=checkInDate.ToShortDateString(),
                CheckOut=checkOutDate.ToShortDateString(),
                BaseAmount = baseAmount,
                GST = gst,
                TotalAmount = totalAmount,
                RoomCostDetails = roomDetails
            };
            return ServiceResponse<RoomCostResultDto>.Success(res, "Room found");
        }
    }
}
