using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace vm_api_backend_appservice.Hubs
{
    [AllowAnonymous]
    public class VirtualMachineHub : Hub
    {
        private readonly ILogger<VirtualMachineHub> _logger;

        public VirtualMachineHub(ILogger<VirtualMachineHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation($"SignalR client connected: {Context.ConnectionId}, User: {userId}");
            
            // Automatically add all clients to the VM updates group
            await Groups.AddToGroupAsync(Context.ConnectionId, "VmUpdatesGroup");
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User?.Identity?.Name ?? "Anonymous";
            _logger.LogInformation($"SignalR client disconnected: {Context.ConnectionId}, User: {userId}");
            
            // Remove clients from the group when they disconnect
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "VmUpdatesGroup");
            
            await base.OnDisconnectedAsync(exception);
        }

        // Methods that clients can call
        public async Task JoinVmUpdates()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "VmUpdatesGroup");
        }

        public async Task LeaveVmUpdates()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "VmUpdatesGroup");
        }
        
        // Test method clients can call to check connectivity
        public async Task TestConnection()
        {
            string userId = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.Caller.SendAsync("TestConnectionResponse", $"Connection successful at {DateTime.UtcNow}. User: {userId}");
        }
    }
} 