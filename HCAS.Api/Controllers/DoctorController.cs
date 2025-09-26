using HCAS.Domain.Features.Doctors;
using HCAS.Domain.Models.Doctors;
using HCAS.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DoctorController : BaseController
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: api/v1/Doctor
        // Supports pagination, search, and filter by specialization
        [HttpGet]
        public async Task<IActionResult> GetDoctors(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? specializationId = null)
        {
            var result = await _doctorService.GetDoctorsAsync(page, pageSize, search, specializationId);
            return Excute(result);   
        }

        // POST: api/v1/Doctor
        [HttpPost]
        public async Task<IActionResult> RegisterDoctor([FromBody] DoctorsReqModel dto)
        {
            var result = await _doctorService.RegisterDoctorAsync(dto);
            return Excute(result);
        }

        // PUT: api/v1/Doctor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorsReqModel dto)
        {
            var result = await _doctorService.UpdateDoctorAsync(id, dto);
            return Excute(result);
        }

        // DELETE: api/v1/Doctor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            return Excute(result);
        }
    }
}