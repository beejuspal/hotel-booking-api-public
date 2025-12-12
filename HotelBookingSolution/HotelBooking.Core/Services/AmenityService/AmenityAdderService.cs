using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.AmenityDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IAmenityService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.AmenityService
{
    public class AmenityAdderService : IAmenityAdderService
    {
        private readonly IAmenityRepository _amenityRepository;
        public AmenityAdderService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }



        public async Task<ServiceResponse<AmenityInsertResponseDTO>> CreateAmenityAsync(AmenityInsertDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<AmenityInsertResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid request data"

                       );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);

            // Duplicate check
            bool exists = await _amenityRepository.AmenityExistsAsync(request.Name,0);
            if (exists)
            {
                return ServiceResponse<AmenityInsertResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Amenity name already exists."

                      );
            }

            // Create entity
            var amenity = new Amenity
            {
                Name = request.Name,
              
                Description = request.Description,
                CreatedDate = DateTime.UtcNow,
                CreatedBy=request.CreatedBy
               
              
            };

            // Save using repository
            int newId = await _amenityRepository.AddAmenityeAsync(amenity);

           
            return ServiceResponse<AmenityInsertResponseDTO>.Success(new AmenityInsertResponseDTO
            {
                AmenityID = newId,
               
            }, "Amenity added successful");
        }

    }
}
