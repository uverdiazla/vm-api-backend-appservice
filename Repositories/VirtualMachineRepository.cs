using Microsoft.EntityFrameworkCore;
using vm_api_backend_appservice.Data;
using vm_api_backend_appservice.Exceptions;
using vm_api_backend_appservice.Models;

namespace vm_api_backend_appservice.Repositories
{
    public class VirtualMachineRepository : IVirtualMachineRepository
    {
        private readonly VmDbContext _dbContext;

        public VirtualMachineRepository(VmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<VirtualMachine>> GetAllVirtualMachinesAsync()
        {
            try
            {
                return await _dbContext.VirtualMachines
                    .Include(vm => vm.OperatingSystem)
                    .Include(vm => vm.Status)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error retrieving virtual machines: {ex.Message}");
            }
        }

        public async Task<VirtualMachine?> GetVirtualMachineByIdAsync(int id)
        {
            try
            {
                var vm = await _dbContext.VirtualMachines
                    .Include(vm => vm.OperatingSystem)
                    .Include(vm => vm.Status)
                    .FirstOrDefaultAsync(vm => vm.Id == id);
                
                if (vm == null)
                {
                    throw new NotFoundException($"Virtual machine with ID {id} not found");
                }
                
                return vm;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error retrieving virtual machine: {ex.Message}");
            }
        }

        public async Task<VirtualMachine> CreateVirtualMachineAsync(VirtualMachine virtualMachine)
        {
            if (virtualMachine == null)
            {
                throw new BadRequestException("Virtual machine cannot be null");
            }
            
            try
            {
                virtualMachine.CreatedAt = DateTime.UtcNow;
                virtualMachine.UpdatedAt = DateTime.UtcNow;
                
                _dbContext.VirtualMachines.Add(virtualMachine);
                await _dbContext.SaveChangesAsync();

                // Get complete VM with relations
                var createdVm = await GetVirtualMachineByIdAsync(virtualMachine.Id);
                if (createdVm == null)
                {
                    throw new NotFoundException($"Failed to retrieve created virtual machine with ID {virtualMachine.Id}");
                }
                
                return createdVm;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                throw new BadRequestException($"Error creating virtual machine: {ex.Message}");
            }
        }

        public async Task<VirtualMachine?> UpdateVirtualMachineAsync(int id, VirtualMachine updatedVm)
        {
            if (updatedVm == null)
            {
                throw new BadRequestException("Updated virtual machine cannot be null");
            }
            
            try
            {
                var vm = await _dbContext.VirtualMachines.FindAsync(id);
                
                if (vm == null)
                {
                    throw new NotFoundException($"Virtual machine with ID {id} not found");
                }

                // Update properties
                vm.Cores = updatedVm.Cores;
                vm.Ram = updatedVm.Ram;
                vm.Disk = updatedVm.Disk;
                vm.OperatingSystemId = updatedVm.OperatingSystemId;
                vm.StatusId = updatedVm.StatusId;
                vm.Description = updatedVm.Description;
                vm.Hostname = updatedVm.Hostname;
                vm.IpAddress = updatedVm.IpAddress;
                vm.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                
                // Get updated VM with relations
                return await GetVirtualMachineByIdAsync(id);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error updating virtual machine: {ex.Message}");
            }
        }

        public async Task<bool> DeleteVirtualMachineAsync(int id)
        {
            try
            {
                var vm = await _dbContext.VirtualMachines.FindAsync(id);
                
                if (vm == null)
                {
                    throw new NotFoundException($"Virtual machine with ID {id} not found");
                }

                _dbContext.VirtualMachines.Remove(vm);
                await _dbContext.SaveChangesAsync();
                
                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error deleting virtual machine: {ex.Message}");
            }
        }
    }
} 