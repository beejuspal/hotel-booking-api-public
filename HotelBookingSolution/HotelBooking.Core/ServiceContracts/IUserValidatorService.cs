using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts
{
    public interface IUserValidatorService
    {
        Task<(bool IsValid, string Message)> ValidateNewUser(CreateUserDTO createUser);
    }
}
