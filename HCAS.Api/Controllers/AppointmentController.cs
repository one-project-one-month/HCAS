using HCAS.Database.AppDbContextModels;
using HCAS.Domain.Features.Appointment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointment;

        public AppointmentController(AppointmentService appointment)
        {
            _appointment = appointment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointmentList()
        {
            var appointments = await _appointment.GetAllAppointments();
            return Ok(appointments);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetAppoinmentById(int id)
        //{
        //    var appoinment = await _appointment.GetAppoinmentById(id);
        //    return Ok(appoinment);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(int patientId, int scheduleId)
        {
            var appointment = await _appointment.CreateAppointment(patientId, scheduleId);
            return Ok(appointment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string newStatus)
        {
            var appointment = await _appointment.UpdateAppointment(appointmentId, newStatus);
            return Ok(appointment);
        }
    }
}