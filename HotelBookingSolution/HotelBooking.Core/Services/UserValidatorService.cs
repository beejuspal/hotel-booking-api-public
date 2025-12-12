using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class UserValidatorService:IUserValidatorService
    {
        private readonly IAuthRepository _authRepository;

        public UserValidatorService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<(bool IsValid, string Message)> ValidateNewUser(CreateUserDTO createUser)
        {
            if (string.IsNullOrWhiteSpace(createUser.Email))
                return (false, "Email is required");

            if (string.IsNullOrWhiteSpace(createUser.Password))
                return (false, "Password is required");

            var existingUser = await _authRepository.GetUserByEmail(createUser.Email);
            if (existingUser != null)
                return (false, "Email already exists");

            return (true, string.Empty);
        }
    }
}
