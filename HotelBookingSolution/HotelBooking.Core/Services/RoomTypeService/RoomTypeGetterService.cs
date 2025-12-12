using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class RoomTypeGetterService : IRoomTypeGetterService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        public RoomTypeGetterService(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<ServiceResponse<PagedDataResultDto<RoomTypeDTO>>> RetrieveAllRoomTypesByHotelIdAsync(int hotelId)
        {
            var roomTypeList = await _roomTypeRepository.RetrieveAllRoomTypAsyncByHotelId(hotelId);


            if (roomTypeList == null || roomTypeList.Count == 0)
                return ServiceResponse<PagedDataResultDto<RoomTypeDTO>>.Fail(HttpStatusCode.NotFound,
                                "No any room type detail found"

                            );


            // Map entity list to DTO list
            var result = roomTypeList.Select(rt => new RoomTypeDTO
            {
                RoomTypeID = rt.RoomTypeID,
                TypeName = rt.TypeName,
                IsActive = rt.IsActive,
                Description = rt.Description,
                AccessibilityFeatures = rt.AccessibilityFeatures,
                HotelId = rt.HotelId,
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<RoomTypeDTO>
            {
                Items = result,
               
            };
            return ServiceResponse<PagedDataResultDto<RoomTypeDTO>>.Success(pagedDtoResult, "Room type retrieve successful");
        }

        public async Task<ServiceResponse<PagedDataResultDto<RoomTypeDTO>>> RetrieveAllRoomTypesWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            var roomTypeList = await _roomTypeRepository.RetrieveAllRoomTypesWithPaginationAsync(pageNumber,pageSize);


            if (roomTypeList.Item1 == null || roomTypeList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<RoomTypeDTO>>.Fail(HttpStatusCode.NotFound,
                                "No any room type detail found"

                            );


            // Map entity list to DTO list
            var result = roomTypeList.Item1.Select(rt => new RoomTypeDTO
            {
                RoomTypeID = rt.RoomTypeID,
                TypeName = rt.TypeName,
                IsActive = rt.IsActive,
                Description = rt.Description,
                AccessibilityFeatures = rt.AccessibilityFeatures,
                HotelId = rt.HotelId,
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<RoomTypeDTO>
            {
                Items = result,
                TotalRecords = roomTypeList.totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(roomTypeList.totalRecords / (double)pageSize)
            };
            return ServiceResponse<PagedDataResultDto<RoomTypeDTO>>.Success(pagedDtoResult, "Room type retrieve successful");
        }

        public async Task<ServiceResponse<RoomTypeDTO>>  RetrieveRoomTypeByIdAsync(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.RetrieveRoomTypeByIdAsync(roomTypeId);

            if (roomType == null)
                return ServiceResponse<RoomTypeDTO>.Fail(HttpStatusCode.NotFound,
                                 "Room type not found"

                             );

            var res= new RoomTypeDTO
            {
                RoomTypeID = roomType.RoomTypeID,
                TypeName = roomType.TypeName,
                IsActive = roomType.IsActive,
                Description = roomType.Description,
                AccessibilityFeatures = roomType.AccessibilityFeatures,
                HotelId = roomType.HotelId,
            };
            return ServiceResponse<RoomTypeDTO>.Success(res, "Room type found");
        }
    }
}
