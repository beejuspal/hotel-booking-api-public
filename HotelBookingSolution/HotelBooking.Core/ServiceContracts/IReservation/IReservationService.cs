using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Core.Services.ReservationService.ReservationService;

namespace HotelBooking.Core.ServiceContracts.IReservation
{
    public interface IReservationService
    {
        Task<ServiceResponse<ReservationResponseDto>> CreateReservationAsync(ReservationCreateRequestDto reservationCreateRequestDto);
        Task<ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>> GetAdminFilteredReservationsAsync(ReservationAdminFilterDto filter, string role, int? HotelId);
        Task<ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>> GetFilteredReservationsByUserAsync(int userId, ReservationFilterDto filter);
        Task<ServiceResponse<int>> CancelReservationAsync(
         int reservationId, int userId, string role);

    }
}
