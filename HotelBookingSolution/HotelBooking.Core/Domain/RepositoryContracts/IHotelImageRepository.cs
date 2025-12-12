using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IHotelImageRepository
    {
        Task<List<HotelImage>> GetByHotelIdAsync(int hotelId);
        Task<HotelImage> AddAsync(HotelImage image);
        Task DeleteAsync(HotelImage image);
    }
}
