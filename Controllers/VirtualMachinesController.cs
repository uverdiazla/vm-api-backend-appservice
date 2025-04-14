using Microsoft.AspNetCore.Mvc;
using vm_api_backend_appservice.Attributes;
using vm_api_backend_appservice.Models.DTOs;
using vm_api_backend_appservice.Services;
using Microsoft.AspNetCore.Authorization;

namespace vm_api_backend_appservice.Controllers
{
    [ApiController]
    [Route("api/vms")]
    [Produces("application/json")]
    [Authorize]
    public class VirtualMachinesController : ControllerBase
    {
        private readonly IVirtualMachineService _vmService;

        public VirtualMachinesController(IVirtualMachineService vmService)
        {
            _vmService = vmService;
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
        public async Task<ActionResult<IEnumerable<VirtualMachineResponseDto>>> GetAllVirtualMachines()
        {
            var vms = await _vmService.GetAllVirtualMachinesAsync();
            return Ok(vms);
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
        public async Task<ActionResult<VirtualMachineResponseDto>> GetVirtualMachine(int id)
        {
            var vm = await _vmService.GetVirtualMachineByIdAsync(id);
            return Ok(vm);
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
        public async Task<ActionResult<VirtualMachineResponseDto>> CreateVirtualMachine([FromBody] CreateVirtualMachineDto createVmDto)
        {
            var vm = await _vmService.CreateVirtualMachineAsync(createVmDto);
            return CreatedAtAction(nameof(GetVirtualMachine), new { id = vm.Id }, vm);
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
        public async Task<ActionResult<VirtualMachineResponseDto>> UpdateVirtualMachine(int id, [FromBody] UpdateVirtualMachineDto updateVmDto)
        {
            var vm = await _vmService.UpdateVirtualMachineAsync(id, updateVmDto);
            return Ok(vm);
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
        public async Task<ActionResult> DeleteVirtualMachine(int id)
        {
            var result = await _vmService.DeleteVirtualMachineAsync(id);
            return Ok(new { success = result, message = "Virtual machine deleted successfully" });
        }
    }
} 