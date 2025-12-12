using CloudinaryDotNet;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.ServiceContracts.IHotelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.HotelService
{
    public class HotelGetterService : IHotelGetterService
    {
        private readonly IHotelRepository _hotelRepository;
       
        public HotelGetterService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
            
        }
        public async Task<ServiceResponse<PagedDataResultDto<HotelDto>>> RetrieveAllHotelsWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            var hotelList = await _hotelRepository.GetAllWithPaginationAsync(pageNumber,pageSize);


            if (hotelList.Item1 == null || hotelList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<HotelDto>>.Fail(HttpStatusCode.NotFound,
                                "No any hotel detail found"

                            );

            // Map entity list to DTO list

            var result = hotelList.Item1.Select(hotel => new HotelDto
            {
                Id = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                Description = hotel.Description,
                //HotelImageUrls = hotel.HotelImgs.Select(img => img.ImageUrl).ToList()
                HotelImageUrls = hotel.HotelImgs.Select(img => new HotelImageDto
                {
                    Id = img.HotelImageId,
                    Url = img.ImageUrl
                }).ToList()
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<HotelDto>
            {
                Items = result,
                TotalRecords = hotelList.totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(hotelList.totalRecords / (double)pageSize)
            };
            return ServiceResponse<PagedDataResultDto<HotelDto>>.Success(pagedDtoResult, "Hotel retrieve successful");
        }

        public async Task<ServiceResponse<PagedDataResultDto<HotelForRoomTypeDto>>> RetrieveAllHotelsAsync()
        {
            var hotelList = await _hotelRepository.GetAllAsync();


            if (hotelList.Item1 == null || hotelList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<HotelForRoomTypeDto>>.Fail(HttpStatusCode.NotFound,
                                "No any hotel detail found"

                            );

            // Map entity list to DTO list

            var result = hotelList.Item1.Select(hotel => new HotelForRoomTypeDto
            {
                Id = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
              
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<HotelForRoomTypeDto>
            {
                Items = result,
                TotalRecords = hotelList.totalRecords,
              
            };
            return ServiceResponse<PagedDataResultDto<HotelForRoomTypeDto>>.Success(pagedDtoResult, "Hotel retrieve successful");
        }

        public async Task<ServiceResponse<HotelDto>> RetrieveHotelByIdAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);

            if (hotel == null)
                return ServiceResponse<HotelDto>.Fail(HttpStatusCode.NotFound,
                                 "hotel detail not found"

                             );

            var hotelRes = new HotelDto
            {
                Id = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                Description = hotel.Description,
                HotelImageUrls = hotel.HotelImgs.Select(img => new HotelImageDto
                {
                    Id = img.HotelImageId,
                    Url = img.ImageUrl
                }).ToList()
            };
            return ServiceResponse<HotelDto>.Success(hotelRes, "Hotel found");
        }

        public async Task<ServiceResponse<PagedDataResultDto<RoomSearchDto>>> GetHotelRoomAmenitiesAsync(
       RoomSearchRequest request)
        {
           var result= await _hotelRepository.GetHotelRoomAmenitiesAsync(request);
            var pagedDtoResult = new PagedDataResultDto<RoomSearchDto>
            {
                Items = result,
                TotalRecords =0,

            };
            return ServiceResponse<PagedDataResultDto<RoomSearchDto>>.Success(pagedDtoResult, "Hotel retrieve successful");
        }

        public async Task<ServiceResponse<PagedDataResultDto<FeaturedHotelDto>>> RetrieveAllFeaturedHotelsWithPaginationAsync(HotelSearchRequestDto requestDto)
        {
            var hotelList = await _hotelRepository.GetFeaturedHotelsAsync(requestDto);


            if (hotelList.Item1 == null || hotelList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<FeaturedHotelDto>>.Fail(HttpStatusCode.NotFound,
                                "No any hotel detail found"

                            );

            // Map entity list to DTO list

            var result = hotelList.Item1.Select(h => new FeaturedHotelDto
            {
                HotelID = h.HotelID,
                HotelName = h.HotelName,
                HotelLocation = h.HotelLocation,
                Description = h.Description,
                StarRating = h.StarRating,
                HotelImages = h.HotelImages.Select(img => new FeaturedHotelImageDto
                {
                    HotelImageID = img.HotelImageID,
                    ImageUrl = img.ImageUrl
                }).ToList(),
                RoomTypes = h.RoomTypes.Select(rt => new FeaturedRoomTypeDto
                {
                    RoomTypeID = rt.RoomTypeID,
                    TypeName = rt.TypeName,
                    AccessibilityFeatures = rt.AccessibilityFeatures,
                    Description = rt.Description,
                    Rooms = rt.Rooms.Select(r => new FeaturedRoomDto
                    {
                        RoomID = r.RoomID,
                        RoomNumber = r.RoomNumber,
                        Price = r.Price,
                        BedType = r.BedType,
                        ViewType = r.ViewType,
                        Status = r.Status,
                        RoomImages = r.RoomImages.Select(img => new FeaturedRoomImageDto
                        {
                            RoomImageID = img.RoomImageID,
                            ImageUrl = img.ImageUrl
                        }).ToList(),
                    }).ToList(),
                    Amenities = rt.Amenities.Select(ra => new FeaturedAmenityDto
                    {
                        AmenityID = ra.AmenityID,
                        Name = ra.Name
                    }).ToList()
                }).ToList()
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<FeaturedHotelDto>
            {
                Items = result,
                TotalRecords = hotelList.totalRecords,
                PageNumber = requestDto.Page,
                PageSize = requestDto.PageSize,
                TotalPages = (int)Math.Ceiling(hotelList.totalRecords / (double)requestDto.PageSize)
            };
            return ServiceResponse<PagedDataResultDto<FeaturedHotelDto>>.Success(pagedDtoResult, "Hotel retrieve successful");
        }
    }
}
