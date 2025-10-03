using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Appoinment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppoinmentController : ControllerBase
    {
        private readonly AppointmentService _appointment;

        public AppoinmentController(AppointmentService appointment)
        {
            _appointment = appointment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppoinmentList()
        {
            var appoinment = await _appointment.GetAllAppointments();
            return Ok(appoinment);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetAppoinmentById(int id)
        //{
        //    var appoinment = await _appointment.GetAppoinmentById(id);
        //    return Ok(appoinment);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateAppoinment(int patientId, int scheduleId) 
        {
            var appoinment = await _appointment.CreateAppointment(patientId,scheduleId);
            return Ok(appoinment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppoinmentStatus(int appointmentId, string newStatus)
        {
            var appoinment = await _appointment.UpdateAppointment(appointmentId, newStatus);
            return Ok(appoinment);
        }
    }
}
