using HCAS.Domain.Features.Specialization;
using HCAS.Domain.Features.Model.Specialization;
using HCAS.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {
        private readonly SpecializationSerivce _specializationSerivce;

        public SpecializationController(SpecializationSerivce specializationSerivce)
        {
            _specializationSerivce = specializationSerivce;        
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecializationList()
        {
            var specializationList = await _specializationSerivce.GetAllSpecializationList();
            return Ok(specializationList.Data);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSpecialization(SpecializationResModel dto)
        {
            var registerSpecialization = await _specializationSerivce.RegisterSpecializations(dto);
            return Ok(registerSpecialization.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSpecialization(SpecializationReqModel dto, int id)
        {
            var updateSpecialization = await _specializationSerivce.UpdateSpecializations(dto,id);
            return Ok(updateSpecialization.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var deleteSpecialization = await _specializationSerivce.DeleteSpecializations(id);
            return Ok(deleteSpecialization.Data);
        }
    }
}
