using Microsoft.AspNetCore.SignalR;
using vm_api_backend_appservice.Hubs;
using vm_api_backend_appservice.Models.DTOs;

namespace vm_api_backend_appservice.Services
{
    public class VirtualMachineNotificationService : IVirtualMachineNotificationService
    {
        private readonly IHubContext<VirtualMachineHub> _hubContext;
        private readonly ILogger<VirtualMachineNotificationService> _logger;

        public VirtualMachineNotificationService(
            IHubContext<VirtualMachineHub> hubContext,
            ILogger<VirtualMachineNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyVmCreatedAsync(VirtualMachineResponseDto vm)
        {
            try
            {
                // Send to the group
                var groupTask = _hubContext.Clients.Group("VmUpdatesGroup").SendAsync("VmCreated", vm);
                
                // Also send to all clients to ensure everyone gets the message
                var allTask = _hubContext.Clients.All.SendAsync("VmCreated", vm);
                
                // Wait for both to complete
                await Task.WhenAll(groupTask, allTask);
                
                _logger.LogInformation($"VM created notification sent: VM ID {vm.Id}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Failed to send VM created notification for VM ID: {vm.Id}");
            }
        }

        public async Task NotifyVmUpdatedAsync(VirtualMachineResponseDto vm)
        {
            try
            {
                // Send to the group
                var groupTask = _hubContext.Clients.Group("VmUpdatesGroup").SendAsync("VmUpdated", vm);
                
                // Also send to all clients to ensure everyone gets the message
                var allTask = _hubContext.Clients.All.SendAsync("VmUpdated", vm);
                
                // Wait for both to complete
                await Task.WhenAll(groupTask, allTask);
                
                _logger.LogInformation($"VM updated notification sent: VM ID {vm.Id}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Failed to send VM updated notification for VM ID: {vm.Id}");
            }
        }

        public async Task NotifyVmDeletedAsync(int vmId)
        {
            try
            {
                // Send to the group
                var groupTask = _hubContext.Clients.Group("VmUpdatesGroup").SendAsync("VmDeleted", vmId);
                
                // Also send to all clients to ensure everyone gets the message
                var allTask = _hubContext.Clients.All.SendAsync("VmDeleted", vmId);
                
                // Wait for both to complete
                await Task.WhenAll(groupTask, allTask);
                
                _logger.LogInformation($"VM deleted notification sent: VM ID {vmId}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Failed to send VM deleted notification for VM ID: {vmId}");
            }
        }
    }
} 