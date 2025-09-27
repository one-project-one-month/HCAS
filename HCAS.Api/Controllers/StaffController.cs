using HCAS.Domain.Features.Staff;
using HCAS.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HCAS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _staffService;
        public StaffController(StaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffListAsync()
        {
            var staffList = await _staffService.GetAllStaffAsync();
            return Ok(staffList.Data);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStaffAsync(StaffReqModel dto)
        {
            var registerStaff = await _staffService.RegisterStaffAsync(dto);
            return Ok(registerStaff.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStaffAsync(StaffReqModel dto)
        {
            var updateStaff = await _staffService.UpdateStaffAsync(dto);
            return Ok(updateStaff.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStaffAsync(int id)
        {
            var deleteStaff = await _staffService.DeleteStaffAsync(id);
            return Ok(deleteStaff.Data);
        }
    }
}