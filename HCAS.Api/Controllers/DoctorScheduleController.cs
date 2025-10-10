using HCAS.Domain.Features.DoctorSchedule;
using HCAS.Domain.Models.DoctorSchedule;
using HCAS.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DoctorScheduleController : BaseController
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
            var result = await _doctorScheduleService.CreateSchedule(dto);
            return Excute(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorSchedule(int id, [FromBody] DoctorScheduleReqModel dto)
        {
            var result = await _doctorScheduleService.UpdateSchedule(id, dto);
            return Excute(result);
        }


        //GET: api/v1/DoctorSchedule

        [HttpGet]
        public async Task<IActionResult> GetSchedules(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null
        )
        {
            var result = await _doctorScheduleService.GetAllSchedules();
            return Excute(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedules(int id)
        {
            var result = await _doctorScheduleService.DeleteSchedule(id);
            return Excute(result);
        }

        [HttpGet("getAvailable")]
        public async Task<IActionResult> GetAvailableSchedules()
        {
            var result = await _doctorScheduleService.GetAvailableSchedules();
            return Excute(result);
        }
    }
}
