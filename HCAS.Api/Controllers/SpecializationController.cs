using HCAS.Domain.Features.Specializations;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SpecializationController : ControllerBase
{
    private readonly SpecializationService _specializationSerivce;

    public SpecializationController(SpecializationService specializationSerivce)
    {
        _specializationSerivce = specializationSerivce;        
    }

    [HttpGet]
    public async Task<IActionResult> GetSpecializationList()
    {
        var specializationList = await _specializationSerivce.GetSpecializationAsync();
        return Ok(specializationList.Data);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterSpecialization(SpecializationReqModel dto)
    {
        var registerSpecialization = await _specializationSerivce.RegisterSpecializationAsync(dto);
        return Ok(registerSpecialization.Data);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSpecialization(int id,SpecializationReqModel dto)
    {
        var updateSpecialization = await _specializationSerivce.UpdateSpecializationAsync(id,dto);
        return Ok(updateSpecialization.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSpecialization(int id)
    {
        var deleteSpecialization = await _specializationSerivce.DeleteSpecializationAsync(id);
        return Ok(deleteSpecialization.Data);
    }
}
