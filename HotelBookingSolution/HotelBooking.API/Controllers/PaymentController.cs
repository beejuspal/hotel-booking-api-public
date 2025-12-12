using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.PaymentDto;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IPayment;
using HotelBooking.Core.ServiceContracts.IReservation;
using HotelBooking.Core.Services.ReservationService;
using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;
using System.Text;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUserGetterService _userGetterService;
        private readonly ILogger<PaymentController> _logger;
        // Sandbox Credentials (FOR TESTING ONLY)
        private readonly string Khalti_SandboxKey = "live_secret_key_68791341fdd94846a146f0457ff7b455";
        private readonly string eSewa_SandboxKey = "8gBm/:&EnhH.1/q";

        // Production Credentials (Replace with your actual credentials)
        private readonly string Khalti_ProductionKey = "your_khalti_live_secret_key";
        private readonly string eSewa_ProductionKey = "your_esewa_live_secret_key";

        // Set to false for production
        private readonly bool sandBoxMode = true;

        private string KhaltiKey => sandBoxMode ? Khalti_SandboxKey : Khalti_ProductionKey;
        private string eSewaKey => sandBoxMode ? eSewa_SandboxKey : eSewa_ProductionKey;
        private readonly string Khalti_SecretKey = "live_secret_key_68791341fdd94846a146f0457ff7b455";


        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IUserGetterService userGetterService)
        {
            _paymentService = paymentService;
            _userGetterService = userGetterService;
            _logger = logger;
        }

        public class KhaltiCustomerInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
        }
        public class KhaltiProductDetail
        {

            public string Name { get; set; } = string.Empty;
            public string Identity { get; set; } = string.Empty;
            public double TotalPrice { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
        }
        public class KhaltiAmountBreakdown
        {


            public string Label { get; set; }

            public double Amount { get; set; }
        }

        public class ApiResponse
        {
            public HttpStatusCode status { get; set; } = HttpStatusCode.OK;
            /// <summary>
            /// Clear message that defines the respose clearly
            /// </summary>
            public string message { get; set; } = string.Empty;
            /// <summary>
            /// Holds the result object to be transmitted
            /// </summary>
            public object data { get; set; }
            public bool success { get; set; } = true;
            public int error_code { get; set; }
        }

        [RoleAuthorization()]
        [HttpPost("Kalti/{reservationID}")]
        public async Task<IActionResult> PayWithKhalti(int reservationID)
        {
            int userId = int.Parse(User.FindFirst("id")?.Value!);
            if (userId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            //var userEmail = User.GetEmail();

            //if (string.IsNullOrEmpty(userEmail))
            //    return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");
         
            //var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            //if (user == null)
            //    return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

           
            var serviceResponse = await _paymentService.InitiateKhaltiPaymentAsync(reservationID, userId);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<KhaltiInitiateResponseDto>.Success(serviceResponse.Data, "Retrieve all reservations successfully!!"));
        }
        [RoleAuthorization()]
        [HttpPut("UpdatePayment/{transactionId}")]
        public async Task<IActionResult> UpdatePayment(string transactionId)
        {
            int userId = int.Parse(User.FindFirst("id")?.Value!);
            if (userId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            //var userEmail = User.GetEmail();

            //if (string.IsNullOrEmpty(userEmail))
            //    return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            //var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            //if (user == null)
            //    return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

          
            var serviceResponse = await _paymentService.UpdatePaymentAsync(transactionId, userId);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(serviceResponse.Data, "Payment failed"));
        }
        [RoleAuthorization()]
        [HttpPost("VerifyKhaltiPayment")]
        public async Task<IActionResult> VerifyKhaltiPayment([FromBody] KhaltiVerifyRequestDto request)
        {
            int userId = int.Parse(User.FindFirst("id")?.Value!);
            if (userId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            //var userEmail = User.GetEmail();

            //if (string.IsNullOrEmpty(userEmail))
            //    return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            //var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            //if (user == null)
            //    return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

            if(request==null) return ErrorResponseHelper.Create(HttpStatusCode.BadRequest, "Request data not found!!");
           
            var serviceResponse = await _paymentService.VerifyKhaltiPaymentAsync(request.TransactionId, request.Amount, userId);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(serviceResponse.Data, serviceResponse.Message));
        }

        [RoleAuthorization(SD.Role_HotelManager)]
        [HttpPut("MakeManualPayment/{reservationId}")]
        public async Task<IActionResult> MakeManualPayment(int reservationId)
        {
          
            var userEmail = User.GetEmail();

            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

            if(!user.HotelID.HasValue) return ErrorResponseHelper.Create(HttpStatusCode.Forbidden, "You are not authorized to complete payment for this hotel.");
         
            var serviceResponse = await _paymentService.CompleteManualPaymentAsync(reservationId, user.HotelID.Value);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(serviceResponse.Data, "Manual Payment success"));
        }

        [RoleAuthorization()]
        [HttpPost("GetAllPayments")]
        public async Task<IActionResult> GetAllPayments([FromBody] PaymentFilterDto filter)
        {
            var userEmail = User.GetEmail();

            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

           
            string role = user.RoleName;
            if (filter.ViewOwn) role = "Guest";
           
            var serviceResponse = await _paymentService.GetFilteredRPaymentsAsync(role, user.HotelID.HasValue? user.HotelID.Value:null,user.UserID,filter);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<PaymentDetailsDto>>.Success(serviceResponse.Data, "Retrieve all payments successfully!!"));
        }
    }
}
