using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.PaymentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IPayment
{
    public interface IPaymentService
    {
        Task<ServiceResponse<KhaltiInitiateResponseDto>> InitiateKhaltiPaymentAsync(int reservationId, int userId);
        Task<ServiceResponse<object>> UpdatePaymentAsync(
        string transactionId, int userId);
        Task<ServiceResponse<object>> VerifyKhaltiPaymentAsync(
       string transactionId, decimal amount, int userId);
        Task<ServiceResponse<object>> CompleteManualPaymentAsync(int reservationId, int ownerHotelId);
        Task<ServiceResponse<PagedDataResultDto<PaymentDetailsDto>>> GetFilteredRPaymentsAsync(string role, int? hotelId, int userId, PaymentFilterDto filter);
    }
}
