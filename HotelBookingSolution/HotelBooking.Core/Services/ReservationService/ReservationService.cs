using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IReservation;
using HotelBooking.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.ReservationService
{
    public class ReservationService : IReservationService
    {

        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
      
        private const string BaseUrl = "https://dev.khalti.com/api/v2/epayment/initiate/";
        private const string KhaltiSecretKey = "key live_secret_key_68791341fdd94846a146f0457ff7b455"; // Move to appsettings later
        public ReservationService(IReservationRepository reservationRepository, IRoomRepository roomRepository)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
          

        }
        public class kha
        {
            [JsonProperty("pidx")]
            public string Pidx { get; set; }

            [JsonProperty("payment_url")]
            public string PaymentUrl { get; set; }

            [JsonProperty("expires_at")]
            public DateTime ExpiresAt { get; set; }
        }

        public async Task<ServiceResponse<ReservationResponseDto>> CreateReservationAsync(
        ReservationCreateRequestDto reservationCreateRequestDto)
        {
            if (reservationCreateRequestDto.roomIds == null || !reservationCreateRequestDto.roomIds.Any())
                return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.BadRequest, "At least one room must be selected.");

            if (reservationCreateRequestDto.checkInDate == default || reservationCreateRequestDto.checkOutDate == default)
                return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.BadRequest, "Check-in and check-out dates are required.");

            if (reservationCreateRequestDto.checkOutDate <= reservationCreateRequestDto.checkInDate)
                return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.BadRequest, "Check-out date must be later than check-in date.");

            int numberOfNights = (reservationCreateRequestDto.checkOutDate - reservationCreateRequestDto.checkInDate).Days;

            try
            {
                await _reservationRepository.BeginTransactionAsync();



                // ✅ Validate rooms
                var rooms = await _roomRepository.RetrieveRoomsByIdsAsync(reservationCreateRequestDto.roomIds);
                if (rooms.Count != reservationCreateRequestDto.roomIds.Count)
                {
                    var missingIds = reservationCreateRequestDto.roomIds.Except(rooms.Select(r => r.RoomID)).ToList();
                    return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.NotFound,
                        $"Rooms not found for IDs: {string.Join(", ", missingIds)}");
                }

                var unavailableRoom = rooms.FirstOrDefault(r => !r.Status.Equals("Available", StringComparison.OrdinalIgnoreCase));
                if (unavailableRoom != null)
                    return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.BadRequest,
                        $"Room '{unavailableRoom.RoomNumber}' is not available.");

                // ✅ Temporarily mark rooms as occupied
                await _roomRepository.UpdateRoomStatusesAsync(reservationCreateRequestDto.roomIds, "Occupied");

                // ✅ Calculate cost
                const decimal GST_RATE = 0.13m;
                decimal baseCost = rooms.Sum(r => r.Price * numberOfNights);
                decimal totalCost = Math.Round(baseCost * (1 + GST_RATE), 2);

                // ✅ Create reservation
                var reservation = new Reservation
                {
                    UserID = reservationCreateRequestDto.UserID,
                    BookingDate = DateTime.Now,
                    CheckInDate = reservationCreateRequestDto.checkInDate,
                    CheckOutDate = reservationCreateRequestDto.checkOutDate,
                    NumberOfNights = numberOfNights,
                    TotalCost = totalCost,
                    Status = "Reserved",
                    CreatedBy = reservationCreateRequestDto.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = ""
                };

                int reservationId = await _reservationRepository.AddReservationAsync(reservation);

                // ✅ Create ReservationRooms
                var reservationRooms = reservationCreateRequestDto.roomIds.Select(id => new ReservationRoom
                {
                    ReservationID = reservationId,
                    RoomID = id,
                    CheckInDate = reservationCreateRequestDto.checkInDate,
                    CheckOutDate = reservationCreateRequestDto.checkOutDate
                }).ToList();

                await _reservationRepository.AddReservationRoomsAsync(reservationRooms);

                var newGuest = new Guest
                {
                    UserID = reservationCreateRequestDto.UserID,
                    FirstName = reservationCreateRequestDto.guest.FirstName,
                    LastName = reservationCreateRequestDto.guest.LastName,
                    Email = reservationCreateRequestDto.guest.Email,
                    Phone = reservationCreateRequestDto.guest.Phone,
                    Address = reservationCreateRequestDto.guest.Address,
                    AgeGroup = reservationCreateRequestDto.guest.AgeGroup,
                    CountryID = 1,
                    StateID = 1,
                    CreatedBy = reservationCreateRequestDto.CreatedBy,
                    ModifiedBy = "",
                    CreatedDate = DateTime.UtcNow

                };

                //await _reservationRepository.AddGuestAsync(newGuest);
                int guestId = await _reservationRepository.AddGuestAsync(newGuest);

                var insertedReservationRooms = await _reservationRepository.GetReservationRoomsByReservationIdAsync(reservationId);
                var reservationGuests = insertedReservationRooms.Select(rr => new RoomReservationGuest
                {
                    ReservationRoomID = rr.ReservationRoomID,
                    GuestID = guestId
                }).ToList();

                //var reservationGuest =  new ReservationGuest
                //{
                //    ReservationRoomID = rr.ReservationRoomID,
                //    GuestID = guestId
                //}).ToList();

                await _reservationRepository.AddReservationGuestAsync(reservationGuests);


                // ✅ Commit
                await _reservationRepository.CommitTransactionAsync();
                var res = new ReservationResponseDto
                {
                    ReservationId = reservationId,
                    Tax = GST_RATE,
                    TotalCost =Convert.ToDouble( totalCost),
                    BaseCost = Convert.ToDouble(baseCost),
                    NumberOfNights = numberOfNights,
                    BookingDate = DateTime.UtcNow,
                    CheckInDate = reservationCreateRequestDto.checkInDate,
                    CheckOutDate = reservationCreateRequestDto.checkOutDate
                };


                return ServiceResponse<ReservationResponseDto>.Success(res, "Reservation created successfully.");
            }
            catch (Exception ex)
            {
                await _reservationRepository.RollbackTransactionAsync();

                // ♻️ Room revert mechanism
                await _roomRepository.UpdateRoomStatusesAsync(reservationCreateRequestDto.roomIds, "Available");

                return ServiceResponse<ReservationResponseDto>.Fail(HttpStatusCode.InternalServerError,
                    $"Reservation creation failed: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>> GetAdminFilteredReservationsAsync(ReservationAdminFilterDto filter,string role,int? HotelId)
        {
            var (reservations, totalCount) = await _reservationRepository.GetAdminFilteredReservationsAsync( filter,role,HotelId);
            var dtoList = reservations.Select(MapReservationToDto).ToList();
            var pagedDtoResult = new PagedDataResultDto<ReservationDetailsDto>
            {
                TotalRecords = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = dtoList,
                TotalPages = (int)Math.Ceiling(dtoList.Count / (double)filter.PageSize)
            };
            return ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>.Success(pagedDtoResult, "Reservations retrieve successful");
        }

        public async Task<ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>> GetFilteredReservationsByUserAsync(int userId, ReservationFilterDto filter)
        {
            var (reservations, totalCount) = await _reservationRepository.GetFilteredReservationsByUserAsync(userId, filter);
            var dtoList = reservations.Select(MapReservationToDto).ToList();
            var pagedDtoResult = new PagedDataResultDto<ReservationDetailsDto>
            {
                TotalRecords = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = dtoList,
                TotalPages = (int)Math.Ceiling(dtoList.Count / (double)filter.PageSize)
            };
            return ServiceResponse<PagedDataResultDto<ReservationDetailsDto>>.Success(pagedDtoResult, "Reservations retrieve successful");

        }

        public async Task<ServiceResponse<int>> CancelReservationAsync(
        int reservationId, int userId, string role)
        {
            await _reservationRepository.BeginTransactionAsync();
            try
            {
                var reservation = await _reservationRepository.GetReservationsByIdAsync(reservationId);
                if (reservation == null)
                {
                    return ServiceResponse<int>.Fail(HttpStatusCode.NotFound, "Reservation not found.");
                }
                //|| "
                if (role.ToLower() != "admin" && role.ToLower() != "manager")
                {
                    if (reservation.UserID != userId)
                    {
                        return ServiceResponse<int>.Fail(HttpStatusCode.Unauthorized, "You are not authorized to cancel reservation.");
                    }
                }
                // Check if checkout date has already passed
                if (reservation.CheckOutDate.Date < DateTime.UtcNow.Date)
                {
                    return ServiceResponse<int>.Fail(HttpStatusCode.BadRequest, "Cannot cancel a reservation after checkout date.");
                }



                // Optional: Prevent cancel if already canceled or completed
                if (reservation.Status == "Cancelled" || reservation.Status == "Completed" || reservation.Status == "Expired")

                    return ServiceResponse<int>.Fail(HttpStatusCode.BadRequest, "Reservation is already finalized.");

                // Update status
                reservation.Status = "Cancelled";
                reservation.ModifiedDate =DateTime.UtcNow;
                _reservationRepository.UpdateReservationAsync(reservation);

                var roomIds = reservation.ReservationRooms.Select(x => x.RoomID).ToList();
                await _roomRepository.UpdateRoomStatusesAsync(roomIds, "Available"); // Track updates only
               
                var cancellation = new Cancellation
                {
                    ReservationID=reservationId,
                    CancellationDate=DateTime.UtcNow,
                    Reason= role.ToLower() == "admin"? "Admin cancelled the reservation." : role.ToLower() == "manager"? "Owner cancelled the reservation" : $"User {userId} cancelled the reservation.",
                    CancellationFee=0,
                    CancellationStatus="Cancelled",
                    CreatedBy=$"User {userId}",
                    CreatedDate=DateTime.UtcNow,
                    ModifiedBy= $"User {userId}"

                };
                await _reservationRepository.AddReservationCancellAsync(cancellation);


                await _reservationRepository.SaveChangesAsync();

                await _reservationRepository.CommitTransactionAsync(); // ✅ Commit if everything succeeded

                return ServiceResponse<int>.Success(reservationId, "Reservation cancelled successfully.");
            }
            catch (Exception ex)
            {
                await _reservationRepository.RollbackTransactionAsync(); // ❌ Rollback everything if one fails
                return ServiceResponse<int>.Fail(HttpStatusCode.NotFound, "Reservation not found.Something error");
            }
        }

        private ReservationDetailsDto MapReservationToDto(Reservation r)
        {
            var hotel = r.ReservationRooms
      .Select(rr => rr.Room.RoomType.Hotel)
      .FirstOrDefault();
            var guests = r.ReservationRooms
        .SelectMany(rr => rr.RoomReservationGuests)
        .Select(rg => rg.Guest)
        .DistinctBy(g => g.GuestID)
        .Select(g => new GuestDto
        {
            GuestID = g.GuestID,
            FullName = $"{g.FirstName} {g.LastName}",
            Email = g.Email,
            Phone = g.Phone,
            AgeGroup = g.AgeGroup
        })
        .ToList();
            return new ReservationDetailsDto
            {
                ReservationID = r.ReservationID,
                BookingDate = r.BookingDate,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                Status = r.Status,
                TotalCost = r.TotalCost,
                NumberOfNights = r.NumberOfNights,
                UserName = r.User?.FullName ?? "",
                Hotel = hotel == null ? null : new HotelDto
                {
                    HotelID = hotel.HotelId,
                    HotelName = hotel.Name,
                    HotelLocation = hotel.Address,
                    StarRating = hotel.StarRating
                },
                Rooms = r.ReservationRooms.Select(rr => new RoomDetailDto
                {
                    RoomID = rr.Room.RoomID,
                    RoomNumber = rr.Room.RoomNumber,
                    Price = rr.Room.Price,
                    BedType = rr.Room.BedType,
                    ViewType = rr.Room.ViewType,
                    RoomTypeName = rr.Room.RoomType.TypeName
                }).ToList(),
                Guests = guests
            };
        }
    }

}
