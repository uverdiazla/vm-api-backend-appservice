using vm_api_backend_appservice.Models;
using vm_api_backend_appservice.Models.DTOs;

namespace vm_api_backend_appservice.Services
{
    public interface IVirtualMachineService
    {
        Task<IEnumerable<VirtualMachineResponseDto>> GetAllVirtualMachinesAsync();
        Task<VirtualMachineResponseDto> GetVirtualMachineByIdAsync(int id);
        Task<VirtualMachineResponseDto> CreateVirtualMachineAsync(CreateVirtualMachineDto createVmDto);
        Task<VirtualMachineResponseDto> UpdateVirtualMachineAsync(int id, UpdateVirtualMachineDto updateVmDto);
        Task<bool> DeleteVirtualMachineAsync(int id);
    }
} 