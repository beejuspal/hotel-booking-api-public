using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomAmenitites;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.ServiceContracts.IRoomAmenities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomAmenityController : ControllerBase
    {
        private readonly IRoomAmenityService _service;

        public RoomAmenityController(IRoomAmenityService service)
        {
            _service = service;
        }

        [HttpGet("GetRoomAmenityByRoomTypeId/{roomTypeId}")]
        public async Task<IActionResult> GetByRoomType(int roomTypeId)
        {
            var serviceResponse = await _service.GetRoomAmenitiesAsync(roomTypeId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<RoomAmeniitiesDto>.Success(serviceResponse.Data, "Retrieve all rooms successfully!!"));
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] RoomAmenityBulkDto dto)
        {
            //await _service.BulkInsertAsync(dto);
            //return Ok("Bulk Insert Successful");
            await _service.BulkInsertAsync(dto);

            //if (!serviceResponse.IsSuccess)
            //    return StatusCode((int)serviceResponse.StatusCode,
            // APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));


            return Ok(APIResultResponseDto<object>.Success(null, "Bulk Insert Successful!!"));
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> BulkUpdate([FromBody] RoomAmenityBulkDto dto)
        {
            await _service.BulkUpdateAsync(dto);
            return Ok(APIResultResponseDto<object>.Success(null, "Bulk update Successful!!"));
        }

        [HttpPut("bulk-delta")]
        public async Task<IActionResult> BulkDelta([FromBody] RoomAmenityBulkDto dto)
        {
            await _service.BulkDeltaAsync(dto);
            return Ok("Delta Sync Successful");
        }
    }

}
