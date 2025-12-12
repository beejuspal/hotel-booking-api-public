using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IHotelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelBooking.Core.Services.HotelService
{
    public class HotelAdderService : IHotelAdderService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly Cloudinary _cloudinary;
        public HotelAdderService(IHotelRepository hotelRepository, Cloudinary cloudinary)
        {
            _hotelRepository = hotelRepository;
            _cloudinary = cloudinary;
        }
        public async Task<ServiceResponse<HotelDto>> CreateHotelAsync(HotelCreateDto dto)
        {
            if (dto == null)
            {
                return ServiceResponse<HotelDto>.Fail(HttpStatusCode.BadRequest,
                           "Invalid request data"

                       );
            }
            // Model validation
            ValidationHelper.ModelValidation(dto);
            // Duplicate check
            bool exists = await _hotelRepository.HotelExistsAsync(dto.Name,0);
            if (exists)
            {
                return ServiceResponse<HotelDto>.Fail(HttpStatusCode.BadRequest,
                          "hotel name already exists."

                      );
            }

            var hotel = new Hotel
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                StarRating = dto.StarRating,
                Description = dto.Description
            };

            if (dto.HotelImgs != null && dto.HotelImgs.Any())
            {
                foreach (var file in dto.HotelImgs.Take(5))
                {
                    if (file == null || file.Length == 0) continue;
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "hotels"
                    };
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    hotel.HotelImgs.Add(new HotelImage
                    {
                        ImageUrl = result.SecureUrl.AbsoluteUri
                    });
                }
            }

            await _hotelRepository.AddAsync(hotel);
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
            return ServiceResponse<HotelDto>.Success(hotelRes, "Hotel added successful");
        }

        public Task<ServiceResponse<Hotel?>> GetHotelByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
