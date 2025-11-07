using System.Threading.Tasks;
using HCAS.Domain.Features.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    public readonly PatientService _patientService;
    
    public PatientController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPatientList()
    {
        var patientList = await _patientService.GetAllPatient();
        return Ok(patientList.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody]PatientReqModel reqModel)
    {
        var patient = await _patientService.RegisterPatient(reqModel);
        return Ok(patient.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientReqModel reqModel)
    {
        var updPatient = await _patientService.UpdatePatient(reqModel,id);

        if (updPatient.Data == null)
        {
            return NotFound("Patient Not Found!");
        }
        return Ok(updPatient.Data); 

    }

    [HttpDelete]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var delPatient = await _patientService.DeletePatient(id);
        if (delPatient.Data == null)
        {
            return NotFound("Patient Not Found");
        }
        return Ok(delPatient.Data);
    }
    
}
