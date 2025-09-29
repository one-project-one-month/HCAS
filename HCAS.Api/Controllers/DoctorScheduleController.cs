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

        [HttpPost("create")]
        public async Task<IActionResult> CreateDoctorSchedule(DoctorScheduleReqModel dto)
        {
            var result = await _doctorScheduleService.CreateSchedule(dto);
            return Excute(result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDoctorSchedule(int id, DoctorScheduleReqModel dto)
        {
            var result = await _doctorScheduleService.UpdateSchedule(id, dto);
            return Excute(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var result = await _doctorScheduleService.GetAllSchedules();
            return Excute(result);
        }

        [HttpDelete("delete/{id}")]
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
