using Microsoft.AspNetCore.Mvc;
using WebApplication5.DTO;
using WebApplication5.Exception;
using WebApplication5.Service;

namespace WebApplication5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionController(IDbService dbservice) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequest request)
    {
        try
        {
            var result = await dbservice.AddPrescriptionAsync(request);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        try
        {
            var result = await dbservice.GetPatientDetailsAsync(id);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}