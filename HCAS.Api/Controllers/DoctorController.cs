using HCAS.Domain.Features.Doctors;
using HCAS.Domain.Features.Model.Doctors;
using HCAS.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorList()
        {
            var result = await _doctorService.GetAllDoctorsList();

            return Ok(result.Data);
        }


        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(DoctorsReqModel dto)
        {
            var registerDoctor = await _doctorService.RegisterDoctor(dto);
            return Ok(registerDoctor.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDoctor(DoctorsReqModel dto)
        {
            var updateDoctor = await _doctorService.UpdateDoctor(dto);
            return Ok(updateDoctor.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var updateDoctor = await _doctorService.DeleteDoctor(id);
            return Ok(updateDoctor.Data);
        }

    }
}
