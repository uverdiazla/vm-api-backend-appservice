using vm_api_backend_appservice.Models;

namespace vm_api_backend_appservice.Repositories
{
    public interface IVirtualMachineRepository
    {
        Task<IEnumerable<VirtualMachine>> GetAllVirtualMachinesAsync();
        Task<VirtualMachine?> GetVirtualMachineByIdAsync(int id);
        Task<VirtualMachine> CreateVirtualMachineAsync(VirtualMachine virtualMachine);
        Task<VirtualMachine?> UpdateVirtualMachineAsync(int id, VirtualMachine virtualMachine);
        Task<bool> DeleteVirtualMachineAsync(int id);
    }
} 