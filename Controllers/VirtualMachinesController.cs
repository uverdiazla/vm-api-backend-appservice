using Microsoft.AspNetCore.Mvc;
using vm_api_backend_appservice.Attributes;
using vm_api_backend_appservice.Models.DTOs;
using vm_api_backend_appservice.Services;
using vm_api_backend_appservice.Exceptions;

namespace vm_api_backend_appservice.Controllers
{
    [ApiController]
    [Route("api/vms")]
    [Produces("application/json")]
    public class VirtualMachinesController : ControllerBase
    {
        private readonly IVirtualMachineService _vmService;
        private readonly ILogger<VirtualMachinesController> _logger;

        public VirtualMachinesController(
            IVirtualMachineService vmService,
            ILogger<VirtualMachinesController> logger)
        {
            _vmService = vmService;
            _logger = logger;
        }

        /// <summary>
        /// Get all virtual machines
        /// </summary>
        /// <remarks>
        /// Available for both Admin and Client users
        /// </remarks>
        /// <returns>List of all virtual machines</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VirtualMachineResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<VirtualMachineResponseDto>>> GetAllVirtualMachines()
        {
            try
            {
                var vms = await _vmService.GetAllVirtualMachinesAsync();
                return Ok(vms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all virtual machines");
                return StatusCode(500, new { error = "An error occurred while retrieving virtual machines" });
            }
        }

        /// <summary>
        /// Get a specific virtual machine by its ID
        /// </summary>
        /// <remarks>
        /// Available for both Admin and Client users
        /// </remarks>
        /// <param name="id">The ID of the virtual machine</param>
        /// <returns>The virtual machine details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VirtualMachineResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachineResponseDto>> GetVirtualMachine(int id)
        {
            try
            {
                var vm = await _vmService.GetVirtualMachineByIdAsync(id);
                return Ok(vm);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Virtual machine not found: {Id}", id);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting virtual machine with ID {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the virtual machine" });
            }
        }

        /// <summary>
        /// Create a new virtual machine
        /// </summary>
        /// <remarks>
        /// Available only for Admin users
        /// </remarks>
        /// <param name="createVmDto">The details of the virtual machine to create</param>
        /// <returns>The created virtual machine</returns>
        [HttpPost]
        [AdminOnly]
        [ProducesResponseType(typeof(VirtualMachineResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachineResponseDto>> CreateVirtualMachine([FromBody] CreateVirtualMachineDto createVmDto)
        {
            try
            {
                var vm = await _vmService.CreateVirtualMachineAsync(createVmDto);
                return CreatedAtAction(nameof(GetVirtualMachine), new { id = vm.Id }, vm);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when creating virtual machine");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating virtual machine");
                return StatusCode(500, new { error = "An error occurred while creating the virtual machine" });
            }
        }

        /// <summary>
        /// Update an existing virtual machine
        /// </summary>
        /// <remarks>
        /// Available only for Admin users
        /// </remarks>
        /// <param name="id">The ID of the virtual machine to update</param>
        /// <param name="updateVmDto">The updated details</param>
        /// <returns>The updated virtual machine</returns>
        [HttpPut("{id}")]
        [AdminOnly]
        [ProducesResponseType(typeof(VirtualMachineResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VirtualMachineResponseDto>> UpdateVirtualMachine(int id, [FromBody] UpdateVirtualMachineDto updateVmDto)
        {
            try
            {
                var vm = await _vmService.UpdateVirtualMachineAsync(id, updateVmDto);
                return Ok(vm);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Virtual machine not found when updating: {Id}", id);
                return NotFound(new { error = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when updating virtual machine: {Id}", id);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating virtual machine with ID {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating the virtual machine" });
            }
        }

        /// <summary>
        /// Delete a virtual machine
        /// </summary>
        /// <remarks>
        /// Available only for Admin users
        /// </remarks>
        /// <param name="id">The ID of the virtual machine to delete</param>
        /// <returns>A success message</returns>
        [HttpDelete("{id}")]
        [AdminOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteVirtualMachine(int id)
        {
            try
            {
                var result = await _vmService.DeleteVirtualMachineAsync(id);
                return Ok(new { success = result, message = "Virtual machine deleted successfully" });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Virtual machine not found when deleting: {Id}", id);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting virtual machine with ID {Id}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the virtual machine" });
            }
        }
    }
} 