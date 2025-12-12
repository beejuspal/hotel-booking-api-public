using CloudinaryDotNet;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.PaymentDto;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.ServiceContracts.IPayment;
using HotelBooking.Core.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HotelBooking.Core.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly KhaltiSettings _settings;
        private readonly IReservationRepository _reservationRepository;
        private readonly IPaymentRepository _paymentRepository;

        //private const string BaseUrl = "https://dev.khalti.com/api/v2/epayment/initiate/";
        //private const string KhaltiSecretKey = "key live_secret_key_68791341fdd94846a146f0457ff7b455"; // Move to appsettings later
        public PaymentService(IReservationRepository reservationRepository, IPaymentRepository paymentRepository, IOptions<KhaltiSettings> options)
        {
            _reservationRepository = reservationRepository;
            _paymentRepository = paymentRepository;
            _settings = options.Value;


        }
        public async Task<ServiceResponse<KhaltiInitiateResponseDto>> InitiateKhaltiPaymentAsync(int reservationId, int userId)
        {
            var reservation = await _reservationRepository.GetReservationsByIdAsync(reservationId);


            if (reservation == null)
                return ServiceResponse<KhaltiInitiateResponseDto>.Fail(HttpStatusCode.NotFound,
                                 "No any reservation detail found");

            if (reservation.UserID != userId) return ServiceResponse<KhaltiInitiateResponseDto>.Fail(HttpStatusCode.Unauthorized,
                              "You are not auhtorized to payment");

            // Check if checkout date has already passed
            if (reservation.CheckOutDate.Date < DateTime.UtcNow.Date)
            {
                return ServiceResponse<KhaltiInitiateResponseDto>.Fail(HttpStatusCode.BadRequest, "Cannot pay after checkout date.");
            }



            // Optional: Prevent cancel if already canceled or completed
            if (reservation.Status == "Cancelled" || reservation.Status == "Completed" || reservation.Status == "Expired")

                return ServiceResponse<KhaltiInitiateResponseDto>.Fail(HttpStatusCode.BadRequest, "Reservation is already finalized.");
            // 💵 Calculate total
            decimal baseAmount = reservation.ReservationRooms.Sum(rr => rr.Room.Price);
            decimal gst = baseAmount * 0.13m;
            decimal totalAmount = baseAmount + gst;



            // 🔗 Khalti API Setup


            var payload = new
            {
                return_url = _settings.ReturnUrl,
                website_url = _settings.WebSiteUrl,
                amount = (int)(reservation.TotalCost * 100), // Amount in paisa
                purchase_order_id = $"RES-{reservation.ReservationID}",
                purchase_order_name = "Hotel Reservation",
                customer_info = new
                {
                    name = reservation.User.FullName,
                    email = reservation.User.Email,
                    phone = "98989898"
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", _settings.SecretKey); // 🔐 Replace this

            var apiUrl = _settings.BaseUrl + _settings.PaymentUrl;

            var response = await client.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Khalti initiation failed: {responseContent}");

            // Khalti returns a JSON with payment_url (redirect link)
            var responseData = JsonConvert.DeserializeObject<KhaltiInitiateResponseDto>(responseContent);
            if (responseData != null)
            {
                if (!string.IsNullOrEmpty(responseData.PaymentUrl))
                {
                    var payment = new Payment
                    {
                        ReservationID = reservationId,
                        Amount = reservation.ReservationRooms.Sum(x => x.Room.Price * reservation.NumberOfNights), // total base amount
                        GST = reservation.TotalCost - reservation.ReservationRooms.Sum(x => x.Room.Price * reservation.NumberOfNights), // GST
                        TotalAmount = reservation.TotalCost,
                        PaymentDate = DateTime.UtcNow,
                        PaymentMethod = "Khalti",
                        PaymentStatus = "Pending",
                        TransactionId = responseData.Pidx,
                        FailureReason = ""


                    };
                    await _paymentRepository.AddPaymentAsync(payment);

                    return ServiceResponse<KhaltiInitiateResponseDto>.Success(responseData, "Hotel retrieve successful");
                }
            }
            return ServiceResponse<KhaltiInitiateResponseDto>.Fail(HttpStatusCode.BadRequest, "payment not success");

        }

        public async Task<ServiceResponse<object>> UpdatePaymentAsync(
        string transactionId, int userId)
        {

            var payment = await _paymentRepository.GetPaymentsByTransactionIdAsync(transactionId);

            if (payment == null) return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Payment not found.");


            if (payment.Reservation.UserID != userId) return ServiceResponse<object>.Fail(HttpStatusCode.Unauthorized, "You are not authorized to update payment.");


            payment.PaymentStatus = "Failed";
            payment.FailureReason = "Either user cancel the payment or payment failed.";

            await _paymentRepository.UpdatePaymentStatusAsync(payment);


            return ServiceResponse<object>.Success(null, "Payment cancelled.");

        }
        public async Task<ServiceResponse<object>> VerifyKhaltiPaymentAsync(
       string transactionId, decimal amount, int userId)
        {
            await _reservationRepository.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(transactionId))
                    return ServiceResponse<object>.Fail(HttpStatusCode.BadRequest, "Invalid transaction id.");

                var payment = await _paymentRepository.GetPaymentsByTransactionIdAsync(transactionId);

                if (payment == null) return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Payment not found.");

                if (payment.TotalAmount * 100 != amount) return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Invalid amount");

                if (payment.Reservation.UserID != userId) return ServiceResponse<object>.Fail(HttpStatusCode.Unauthorized, "You are not authorized to update payment.");



                var apiUrl = _settings.BaseUrl + _settings.VerifyUrl;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", _settings.SecretKey);

                var payload = new { pidx = transactionId };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ServiceResponse<object>.Fail(HttpStatusCode.Unauthorized, "Verification faield");

                var result = JsonConvert.DeserializeObject<KhaltiVerifyResponseDto>(responseContent);


                var reservation = await _reservationRepository.GetReservationsByIdAsync(payment.ReservationID);
                // ✅ If success, mark payment & reservation as completed
                if (result?.Status == "Completed")
                {
                    payment.PaymentStatus = "Completed";

                    reservation.Status = "Completed";
                    reservation.ModifiedDate = DateTime.UtcNow;

                }
                else
                {
                    payment.PaymentStatus = "Failed";
                    payment.FailureReason = result.Status;

                    reservation.Status = "Failed";
                    reservation.ModifiedDate = DateTime.UtcNow;
                }
                await _paymentRepository.UpdatePaymentStatusAsync(payment);
                _reservationRepository.UpdateReservationAsync(reservation);

                await _reservationRepository.SaveChangesAsync();

                await _reservationRepository.CommitTransactionAsync(); // ✅ Commit if everything succeeded

                return ServiceResponse<object>.Success(transactionId, "Payment success.");
            }
            catch (Exception ex)
            {
                await _reservationRepository.RollbackTransactionAsync(); // ❌ Rollback everything if one fails
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Reservation not found.Something error");
            }

        }

        public async Task<ServiceResponse<object>> CompleteManualPaymentAsync(int reservationId, int ownerHotelId)
        {
            await _reservationRepository.BeginTransactionAsync();
            try
            {
                // 1️⃣ Get reservation
                var reservation = await _reservationRepository.GetReservationsByIdAsync(reservationId);

                if (reservation == null)
                    return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Reservation not found.");

                // 2️⃣ Check if hotel admin is authorized


                if (ownerHotelId != reservation.ReservationRooms.First().Room.RoomType.HotelId)
                    return ServiceResponse<object>.Fail(HttpStatusCode.Forbidden, "You are not authorized to complete payment for this hotel.");
                // Check if checkout date has already passed
                if (reservation.CheckOutDate.Date < DateTime.UtcNow.Date)
                {
                    return ServiceResponse<object>.Fail(HttpStatusCode.BadRequest, "Cannot pay after checkout date.");
                }



                // Optional: Prevent cancel if already canceled or completed
                if (reservation.Status == "Cancelled" || reservation.Status == "Completed" || reservation.Status == "Expired")

                    return ServiceResponse<object>.Fail(HttpStatusCode.BadRequest, "Reservation is already finalized.");
                // 3️⃣ Create Payment record
                decimal baseAmount = reservation.ReservationRooms.Sum(rr => rr.Room.Price);
                decimal gst = baseAmount * 0.13m;
                decimal totalAmount = baseAmount + gst;
                var payment = new Payment
                {
                    ReservationID = reservation.ReservationID,
                    Amount = baseAmount, // or calculate if needed
                    GST = gst, // example GST
                    TotalAmount = totalAmount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = "Manual", // "Manual" or "Cash"
                    PaymentStatus = "Completed",
                    FailureReason = "",
                    TransactionId = ""
                };
                await _paymentRepository.AddPaymentAsync(payment);


                // 4️⃣ Update reservation status
                reservation.Status = "Completed";


                _reservationRepository.UpdateReservationAsync(reservation);

                await _reservationRepository.SaveChangesAsync();

                await _reservationRepository.CommitTransactionAsync(); // ✅ Commit if everything succeeded

                return ServiceResponse<object>.Success(null, "Payment completed manually and reservation marked as completed.");

            }
            catch (Exception ex)
            {
                await _reservationRepository.RollbackTransactionAsync(); // ❌ Rollback everything if one fails
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound, "Reservation not found.Something error");
            }

        }

        public async Task<ServiceResponse<PagedDataResultDto<PaymentDetailsDto>>> GetFilteredRPaymentsAsync(string role,int? hotelId,int userId, PaymentFilterDto filter)
        {
            var (payments, totalCount) = await _paymentRepository.GetPaymentsAsync(role,userId,hotelId, filter);
            var dtoList = payments.Select(MapPaymentToDto).ToList();
            var pagedDtoResult = new PagedDataResultDto<PaymentDetailsDto>
            {
                TotalRecords = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = dtoList,
                TotalPages = (int)Math.Ceiling(dtoList.Count / (double)filter.PageSize)
            };
            return ServiceResponse<PagedDataResultDto<PaymentDetailsDto>>.Success(pagedDtoResult, "Payments retrieve successful");

        }
        private PaymentDetailsDto MapPaymentToDto(Payment p)
        {
            var hotel = p.Reservation.ReservationRooms
      .Select(rr => rr.Room.RoomType.Hotel)
      .FirstOrDefault();
            var guests = p.Reservation.ReservationRooms
        .SelectMany(rr => rr.RoomReservationGuests)
        .Select(rg => rg.Guest)
        .DistinctBy(g => g.GuestID)
        .Select(g => new DTO.PaymentDto.GuestDto
        {
            GuestID = g.GuestID,
            FullName = $"{g.FirstName} {g.LastName}",
            Email = g.Email,
            Phone = g.Phone,
            AgeGroup = g.AgeGroup
        })
        .ToList();
            return new PaymentDetailsDto
            {
                PaymentID=p.PaymentID,
                ReservationID = p.ReservationID,
                PaymentDate = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                TotalAmount = p.TotalAmount,
                GST = p.GST,
                FailureReason = p.FailureReason,
                TransactionId=p.TransactionId,
                UserName = p.Reservation.User?.FullName ?? "",
                Hotel = hotel == null ? null : new DTO.PaymentDto.HotelDto
                {
                    HotelID = hotel.HotelId,
                    HotelName = hotel.Name,
                    HotelLocation = hotel.Address,
                    StarRating = hotel.StarRating
                },
                Rooms = p.Reservation.ReservationRooms.Select(rr => new DTO.PaymentDto.RoomDetailDto
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
