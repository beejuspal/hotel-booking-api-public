using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.PaymentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IPaymentRepository
    {
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentStatusAsync(Payment payment);
        Task<Payment> GetPaymentsByTransactionIdAsync(string transactionId);
        Task<(List<Payment> Payments, int TotalCount)> GetPaymentsAsync(string role, int userId, int? hotelId, PaymentFilterDto filter);
        Task<List<Payment>> GePaymentByStatus(int userId, string status);
    }
}
