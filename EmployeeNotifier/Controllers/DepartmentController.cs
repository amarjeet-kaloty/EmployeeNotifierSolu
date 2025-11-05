using EmployeeNotifier.DTOs;
using EmployeeNotifier.Service_Invocation;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeNotifier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentEmployeeLookupService _lookupService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(DepartmentEmployeeLookupService lookupService, ILogger<DepartmentController> logger)
        {
            _lookupService = lookupService;
            _logger = logger;
        }

        [HttpPost("ProcessNewHire")]
        [ProducesResponseType(typeof(ReadEmployeeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReadEmployeeDTO>> ProcessNewHire([FromBody] EmployeeIdRequest request)
        {
            _logger.LogInformation("Department received the request to process new hire with ID: {id}.", request.EmployeeId);

            string? authorizationToken = Request.Headers.Authorization.FirstOrDefault();
            _logger.LogInformation($"Authorization token: {authorizationToken}");
            var fullemployeeDetails = await _lookupService.GetEmployeeDetails(request.EmployeeId, authorizationToken);
            if (fullemployeeDetails == null) 
            {
                _logger.LogWarning("Employee details not found using Service Invocation for ID: {id}.", request.EmployeeId);
                return NotFound($"Employee with the ID: {request.EmployeeId} could not be retrived from Employee Management Service.");
            }

            _logger.LogInformation("Successfully retrieved full details for employee:" +
                $"Employee ID: {fullemployeeDetails.Id}," +
                $"Employee Name: {fullemployeeDetails.Name}," +
                $"Employee Address: {fullemployeeDetails.Address}," +
                $"Employee Contact # {fullemployeeDetails.Phone}," +
                $"Employee Email: {fullemployeeDetails.Email}");

            return Ok(fullemployeeDetails);
        }
    }

    public class EmployeeIdRequest
    {
        public string EmployeeId { get; set; }
    }
}
