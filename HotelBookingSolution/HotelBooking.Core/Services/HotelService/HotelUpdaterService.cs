using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Mapper;
using HotelBooking.Core.ServiceContracts.IHotelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.HotelService
{
    public class HotelUpdaterService : IHotelUpdaterService
    {
        private readonly IHotelRepository _hotelRepository;

        private readonly IImageStorageService _imageStorage;

        public HotelUpdaterService(IHotelRepository hotelRepository, IImageStorageService imageStorage)
        {
            _hotelRepository = hotelRepository; 
            _imageStorage = imageStorage;

        }

        public async Task<ServiceResponse<HotelDto>> UpdateHotelAsync(int id, HotelCreateDto dto)
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
            bool exists = await _hotelRepository.HotelExistsAsync(dto.Name, id);
            if (exists)
            {
                return ServiceResponse<HotelDto>.Fail(HttpStatusCode.BadRequest,
                          "Hotel name already exists."

                      );
            }
            var hotel = await _hotelRepository.GetByIdAsync(id);

            if (hotel == null) return ServiceResponse<HotelDto>.Fail(HttpStatusCode.BadRequest,
                          "Hotel detail not found."

                      );

            //hotel.Name = dto.Name;
            //hotel.Address = dto.Address;
            //hotel.City = dto.City;
            //hotel.Country = dto.Country;
            //hotel.PhoneNumber = dto.PhoneNumber;
            //hotel.Email = dto.Email;
            //hotel.StarRating = dto.StarRating;
            //hotel.Description = dto.Description;

            //// Optionally remove old images if needed
            //// Here we add new images
            //// 🟢 Handle images
            //// Step 1: Remove images not in existingImgs


            //// ✅ Handle images
            //var existingUrls = dto.ExistingImgs ?? new List<string>();

            //// Step 1: Find deleted images
            //var deletedImgs = hotel.HotelImgs
            //    .Where(img => !existingUrls.Contains(img.ImageUrl))
            //    .ToList();

            //foreach (var delImg in deletedImgs)
            //{
            //    // Remove from Cloudinary
            //    var publicId = GetPublicIdFromUrl(delImg.ImageUrl);
            //    if (!string.IsNullOrEmpty(publicId))
            //    {
            //        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            //    }

            //    // Remove from DB
            //    await _hotelRepository.DeleteHotelImageAsync(delImg);
            //}
            //if (dto.HotelImgs != null && dto.HotelImgs.Any())
            //{
            //    foreach (var file in dto.HotelImgs.Take(5))
            //    {
            //        if (file == null || file.Length == 0) continue;

            //        using var stream = file.OpenReadStream();
            //        var uploadParams = new ImageUploadParams
            //        {
            //            File = new FileDescription(file.FileName, stream),
            //            Folder = "hotels"
            //        };
            //        var result = await _cloudinary.UploadAsync(uploadParams);

            //        hotel.HotelImgs.Add(new HotelImage
            //        {
            //            ImageUrl = result.SecureUrl.AbsoluteUri
            //        });
            //    }
            //}
            //await _hotelRepository.UpdateAsync(hotel);
            //var hotelRes = new HotelDto
            //{
            //    Id = hotel.HotelId,
            //    Name = hotel.Name,
            //    Address = hotel.Address,
            //    City = hotel.City,
            //    Country = hotel.Country,
            //    PhoneNumber = hotel.PhoneNumber,
            //    Email = hotel.Email,
            //    StarRating = hotel.StarRating,
            //    Description = hotel.Description,
            //    HotelImageUrls = hotel.HotelImgs.Select(img => new HotelImageDto
            //    {
            //        Id = img.HotelImageId,
            //        Url = img.ImageUrl
            //    }).ToList()
            //};
            //return ServiceResponse<HotelDto>.Success(hotelRes, "Hotel updated successful");
            // Update hotel info
            UpdateHotelProperties(hotel, dto);

            // Handle images
            await HandleImagesAsync(hotel, dto);

            await _hotelRepository.UpdateAsync(hotel);

            return ServiceResponse<HotelDto>.Success(HotelMapper.ToDto(hotel), "Hotel updated successfully");
        }
        private void UpdateHotelProperties(Hotel hotel, HotelCreateDto dto)
        {
            hotel.Name = dto.Name;
            hotel.Address = dto.Address;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.PhoneNumber = dto.PhoneNumber;
            hotel.Email = dto.Email;
            hotel.StarRating = dto.StarRating;
            hotel.Description = dto.Description;
        }

        private async Task HandleImagesAsync(Hotel hotel, HotelCreateDto dto)
        {
            var existingUrls = dto.ExistingImgs ?? new List<string>();

            // Delete removed images
            foreach (var delImg in hotel.HotelImgs.Where(img => !existingUrls.Contains(img.ImageUrl)).ToList())
            {
                await _imageStorage.DeleteAsync(delImg.ImageUrl);
                await _hotelRepository.DeleteHotelImageAsync(delImg);
            }

            // Upload new images
            if (dto.HotelImgs != null)
            {
                foreach (var file in dto.HotelImgs.Take(5))
                {
                    var uploadedUrl = await _imageStorage.UploadAsync(file, "hotels");
                    hotel.HotelImgs.Add(new HotelImage { ImageUrl = uploadedUrl });
                }
            }
        }

    }
}
