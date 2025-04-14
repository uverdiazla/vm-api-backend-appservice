using System.Threading.Tasks;
using vm_api_backend_appservice.Models.DTOs;

namespace vm_api_backend_appservice.Services
{
    public interface IVirtualMachineNotificationService
    {
        Task NotifyVmCreatedAsync(VirtualMachineResponseDto vm);
        Task NotifyVmUpdatedAsync(VirtualMachineResponseDto vm);
        Task NotifyVmDeletedAsync(int vmId);
    }
} 