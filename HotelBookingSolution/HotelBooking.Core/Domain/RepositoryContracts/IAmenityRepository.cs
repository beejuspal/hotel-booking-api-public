using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IAmenityRepository
    {
        Task<int> AddAmenityeAsync(Amenity amenity);
        Task<int> UpdateAmenityAsync(Amenity amenity);
        Task DeleteAmenityAsync(Amenity amenity);
        Task<bool> AmenityExistsAsync(string amenityName, int amenityId);

        Task<(List<Amenity>, int totalRecords)> RetrieveAllAmenityWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
  
        Task<Amenity?> RetrieveAmenityByIdAsync(int amenityId);
        Task<List<Amenity>> RetrieveAllAmenitiesAsync();

    }
}
