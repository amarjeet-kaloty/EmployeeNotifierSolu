using Dapr.Client;
using EmployeeNotifier.DTOs;

namespace EmployeeNotifier.Controllers
{
    public class DepartmentEmployeeLookupService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<DepartmentEmployeeLookupService> _logger;

        private const string ManagementAppId = "employeeservice";

        public DepartmentEmployeeLookupService(DaprClient daprClient, ILogger<DepartmentEmployeeLookupService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        /// <summary>
        /// Uses Dapr Service Invocation to fetch full employee details from the Employee Management service.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee to retrieve.</param>
        /// <returns>The full employee DTO, or null if not found.</returns>
        public async Task<ReadEmployeeDTO> GetEmployeeDetails(Guid employeeId)
        {
            string methodPath = $"api/Employee/{employeeId}";

            try
            {
                _logger.LogDebug("Delegating employee detail query to App ID '{ManagementAppId}' at path '{MethodPath}' for ID: {EmployeeId}",
                    ManagementAppId, methodPath, employeeId);

                return await _daprClient.InvokeMethodAsync<ReadEmployeeDTO>(
                    ManagementAppId,
                    methodPath);
            }
            catch (InvocationException ex)
            {
                _logger.LogWarning("Employee ID {EmployeeId} not found in Management Service (404 response).", employeeId);
                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during Dapr service invocation to {ManagementAppId} for Employee ID {EmployeeId}.",
                    ManagementAppId, employeeId);
                throw;
            }
        }
    }
}
