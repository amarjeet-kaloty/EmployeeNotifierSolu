using Dapr;
using EmployeeNotifier.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeNotifier.Controllers
{
    [ApiController]
    [Route("api/subscriptions/employee")]
    public class EmployeeSubscriptionController : Controller
    {
        private readonly ILogger<EmployeeSubscriptionController> _logger;

        public EmployeeSubscriptionController(ILogger<EmployeeSubscriptionController> logger)
        {
            _logger = logger;
        }

        [Topic("rabbitmq-pubsub", "employee_events")]
        [HttpPost("EmployeeCreated")]
        public ActionResult HandleEmployeeCreatedEvent([FromBody] ReadEmployeeDTO employeeData)
        {
            _logger.LogInformation($" [=>] Dapr Received Employee Created Event for Employee:" +
                $" {employeeData.Id}, {employeeData.Name}, {employeeData.Address}, {employeeData.Email}, {employeeData.Phone}");

            return Ok($"Event successfully handled for employee creation.");
        }
    }
}
