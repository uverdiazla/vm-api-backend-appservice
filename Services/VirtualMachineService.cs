using vm_api_backend_appservice.Data;
using vm_api_backend_appservice.Exceptions;
using vm_api_backend_appservice.Models;
using vm_api_backend_appservice.Models.DTOs;
using vm_api_backend_appservice.Repositories;

namespace vm_api_backend_appservice.Services
{
    public class VirtualMachineService : IVirtualMachineService
    {
        private readonly IVirtualMachineRepository _vmRepository;
        private readonly VmDbContext _dbContext;
        private readonly IVirtualMachineNotificationService _notificationService;

        public VirtualMachineService(
            IVirtualMachineRepository vmRepository, 
            VmDbContext dbContext,
            IVirtualMachineNotificationService notificationService)
        {
            _vmRepository = vmRepository;
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<VirtualMachineResponseDto>> GetAllVirtualMachinesAsync()
        {
            var vms = await _vmRepository.GetAllVirtualMachinesAsync();
            return vms.Select(MapToResponseDto);
        }

        public async Task<VirtualMachineResponseDto> GetVirtualMachineByIdAsync(int id)
        {
            var vm = await _vmRepository.GetVirtualMachineByIdAsync(id);
            
            if (vm == null)
            {
                throw new NotFoundException($"Virtual machine with ID {id} not found");
            }
            
            return MapToResponseDto(vm);
        }

        public async Task<VirtualMachineResponseDto> CreateVirtualMachineAsync(CreateVirtualMachineDto createVmDto)
        {
            // Validate that OS and Status exist
            await ValidateOperatingSystemIdAsync(createVmDto.OperatingSystemId);
            await ValidateStatusIdAsync(createVmDto.StatusId);
            
            var vm = new VirtualMachine
            {
                Name = createVmDto.Name,
                Cores = createVmDto.Cores,
                Ram = createVmDto.Ram,
                Disk = createVmDto.Disk,
                OperatingSystemId = createVmDto.OperatingSystemId,
                StatusId = createVmDto.StatusId,
                Description = createVmDto.Description,
                Hostname = createVmDto.Hostname,
                IpAddress = createVmDto.IpAddress
            };

            var createdVm = await _vmRepository.CreateVirtualMachineAsync(vm);
            
            if (createdVm == null)
            {
                throw new BadRequestException("Failed to create virtual machine");
            }
            
            var responseDto = MapToResponseDto(createdVm);
            
            // Notify clients
            await _notificationService.NotifyVmCreatedAsync(responseDto);
            
            return responseDto;
        }

        public async Task<VirtualMachineResponseDto> UpdateVirtualMachineAsync(int id, UpdateVirtualMachineDto updateVmDto)
        {
            // Validate that OS and Status exist
            await ValidateOperatingSystemIdAsync(updateVmDto.OperatingSystemId);
            await ValidateStatusIdAsync(updateVmDto.StatusId);
            
            // We need the original VM to update it properly
            var existingVm = await _vmRepository.GetVirtualMachineByIdAsync(id);
            if (existingVm == null)
            {
                throw new NotFoundException($"Virtual machine with ID {id} not found");
            }
            
            // Map update properties
            existingVm.Cores = updateVmDto.Cores;
            existingVm.Ram = updateVmDto.Ram;
            existingVm.Disk = updateVmDto.Disk;
            existingVm.OperatingSystemId = updateVmDto.OperatingSystemId;
            existingVm.StatusId = updateVmDto.StatusId;
            existingVm.Description = updateVmDto.Description;
            existingVm.Hostname = updateVmDto.Hostname;
            existingVm.IpAddress = updateVmDto.IpAddress;
            existingVm.UpdatedAt = DateTime.UtcNow;

            var updatedVm = await _vmRepository.UpdateVirtualMachineAsync(id, existingVm);
            
            if (updatedVm == null)
            {
                throw new BadRequestException($"Failed to update virtual machine with ID {id}");
            }
            
            var responseDto = MapToResponseDto(updatedVm);
            
            // Notify clients
            await _notificationService.NotifyVmUpdatedAsync(responseDto);
            
            return responseDto;
        }

        public async Task<bool> DeleteVirtualMachineAsync(int id)
        {
            var result = await _vmRepository.DeleteVirtualMachineAsync(id);
            
            if (result)
            {
                // Notify clients
                await _notificationService.NotifyVmDeletedAsync(id);
            }
            
            return result;
        }

        private VirtualMachineResponseDto MapToResponseDto(VirtualMachine vm)
        {
            var vmOs = vm.OperatingSystem;
            var vmStatus = vm.Status;
            
            // Load related entities if not loaded
            if (vmOs == null)
            {
                vmOs = _dbContext.OperatingSystems.Find(vm.OperatingSystemId);
            }
            
            if (vmStatus == null)
            {
                vmStatus = _dbContext.Statuses.Find(vm.StatusId);
            }
            
            return new VirtualMachineResponseDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Cores = vm.Cores,
                Ram = vm.Ram,
                Disk = vm.Disk,
                OperatingSystem = new OperatingSystemDto
                {
                    Id = vmOs?.Id ?? 0,
                    Name = vmOs?.Name ?? string.Empty,
                    Version = vmOs?.Version ?? string.Empty
                },
                Status = new StatusDto
                {
                    Id = vmStatus?.Id ?? 0,
                    Name = vmStatus?.Name ?? string.Empty,
                    ColorCode = vmStatus?.ColorCode ?? string.Empty
                },
                Description = vm.Description,
                Hostname = vm.Hostname,
                IpAddress = vm.IpAddress,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = vm.UpdatedAt
            };
        }
        
        private async Task ValidateOperatingSystemIdAsync(int operatingSystemId)
        {
            var os = await _dbContext.OperatingSystems.FindAsync(operatingSystemId);
            if (os == null)
            {
                throw new BadRequestException($"Operating system with ID {operatingSystemId} does not exist");
            }
        }
        
        private async Task ValidateStatusIdAsync(int statusId)
        {
            var status = await _dbContext.Statuses.FindAsync(statusId);
            if (status == null)
            {
                throw new BadRequestException($"Status with ID {statusId} does not exist");
            }
        }
    }
} 