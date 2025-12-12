using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.HotelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Mapper
{
    public static class HotelMapper
    {
        public static HotelDto ToDto(Hotel hotel)
        {
            return new HotelDto
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
        }
    }

}
