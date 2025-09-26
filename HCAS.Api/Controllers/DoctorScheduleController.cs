using HCAS.Domain.Features.DoctorSchedules;
using HCAS.Domain.Features.Model.DoctorSchedules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly DoctorScheduleService _doctorScheduleService;

        public DoctorScheduleController(DoctorScheduleService doctorScheduleService)
        {
            _doctorScheduleService = doctorScheduleService;
        }

        //POST: api/v1/DoctorSchedule
        [HttpPost]
        public async Task<IActionResult> CreateDoctorSchedule([FromBody] DoctorScheduleReqModel dto)
        {
            var result = await _doctorScheduleService.CreateScheduleAsync(dto);

            if (result.IsValidationError)
                return BadRequest(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorSchedule(int id, [FromBody] DoctorScheduleReqModel dto)
        {
            var result = await _doctorScheduleService.UpdateScheduleAsync(id, dto);
            if (result.IsValidationError)
                return BadRequest(result.Message);

            if (result.IsNotFound)
                return NotFound(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }


        //GET: api/v1/DoctorSchedule

        [HttpGet]
        public async Task<IActionResult> GetSchedules(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null
        )
        {
            var result = await _doctorScheduleService.GetSchedulesAsyn(page,pageSize,search);
            if (result.IsNotFound)
                return NotFound(result.Message);
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedules(int id)
        {
            var result = await _doctorScheduleService.DeleteScheduleAsync(id);
            if (result.IsNotFound)
                return NotFound(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Message);
        }

        [HttpGet("getAvailable")]
        public async Task<IActionResult> GetAvailableSchedules()
        {
            var result = await _doctorScheduleService.GetAvailableSchedules();
            return Ok(result);
        }
    }
}
