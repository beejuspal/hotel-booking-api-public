using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.AmenityDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
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
    public class AmenityUpdaterService : IAmenityUpdaterService
    {
        private readonly IAmenityRepository _amenityRepository;
        public AmenityUpdaterService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }
        public async Task<ServiceResponse<AmenityUpdateResponseDTO>> UpdateAmenityAsync(AmenityUpdateDTO request)
        {
            if (request == null)
            {
                return ServiceResponse<AmenityUpdateResponseDTO>.Fail(HttpStatusCode.BadRequest,
                            "Invalid request"

                        );
            }

            // Model validation
            ValidationHelper.ModelValidation(request);

          
            var amenity = await _amenityRepository.RetrieveAmenityByIdAsync(request.AmenityID);
            if (amenity == null)
            {
                return ServiceResponse<AmenityUpdateResponseDTO>.Fail(HttpStatusCode.NotFound,
                            "Amenity not found"

                        );
            }
            // Duplicate check
            bool exists = await _amenityRepository.AmenityExistsAsync(request.Name, request.AmenityID);
            if (exists)
            {
                return ServiceResponse<AmenityUpdateResponseDTO>.Fail(HttpStatusCode.BadRequest,
                          "Amenity name already exists."

                      );
            }

            // update entity
            amenity.Name = request.Name;

            amenity.Description = request.Description;
            amenity.ModifiedDate = DateTime.UtcNow;
           
            amenity.IsActive = request.IsActive;
           
           
            int updatedId=await _amenityRepository.UpdateAmenityAsync(amenity);
            
            return ServiceResponse<AmenityUpdateResponseDTO>.Success(new AmenityUpdateResponseDTO
            {
                AmenityID = updatedId,
              
            }, "Amenity updated successful");
        }
    }
}
