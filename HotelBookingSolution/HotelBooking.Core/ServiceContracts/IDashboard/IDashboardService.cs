using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.DashboardDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IDashboard
{
    public interface IDashboardService
    {

        Task<ServiceResponse<UserDashboardDto>> GetUserDashboardAsync(int userId);
    }
}
