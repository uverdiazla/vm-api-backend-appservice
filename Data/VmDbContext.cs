using Microsoft.EntityFrameworkCore;
using vm_api_backend_appservice.Models;
using vm_api_backend_appservice.Models.Enums;

namespace vm_api_backend_appservice.Data
{
    public class VmDbContext : DbContext
    {
        public VmDbContext(DbContextOptions<VmDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<VirtualMachine> VirtualMachines { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<VmOperatingSystem> OperatingSystems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@vmapi.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // In production, use proper password hashing
                    Role = Models.Enums.Role.Admin,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            // Seed initial client user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    Name = "Client User",
                    Email = "client@vmapi.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Client123!"), // In production, use proper password hashing
                    Role = Models.Enums.Role.Client,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            // Seed VM statuses
            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "Running", Description = "The virtual machine is running", StatusEnum = VirtualMachineStatus.Running, ColorCode = "#4CAF50" },
                new Status { Id = 2, Name = "Stopped", Description = "The virtual machine is stopped", StatusEnum = VirtualMachineStatus.Stopped, ColorCode = "#F44336" },
                new Status { Id = 3, Name = "Provisioning", Description = "The virtual machine is being provisioned", StatusEnum = VirtualMachineStatus.Provisioning, ColorCode = "#2196F3" },
                new Status { Id = 4, Name = "Failed", Description = "The virtual machine has failed", StatusEnum = VirtualMachineStatus.Failed, ColorCode = "#FF9800" },
                new Status { Id = 5, Name = "Suspended", Description = "The virtual machine is suspended", StatusEnum = VirtualMachineStatus.Suspended, ColorCode = "#9C27B0" },
                new Status { Id = 6, Name = "Maintenance", Description = "The virtual machine is under maintenance", StatusEnum = VirtualMachineStatus.Maintenance, ColorCode = "#607D8B" }
            );
            
            // Seed Operating Systems
            modelBuilder.Entity<VmOperatingSystem>().HasData(
                new VmOperatingSystem { Id = 1, Name = "Windows Server 2019", Description = "Microsoft Windows Server 2019", OsType = OperatingSystemType.WindowsServer2019, Version = "2019", IsActive = true },
                new VmOperatingSystem { Id = 2, Name = "Windows Server 2022", Description = "Microsoft Windows Server 2022", OsType = OperatingSystemType.WindowsServer2022, Version = "2022", IsActive = true },
                new VmOperatingSystem { Id = 3, Name = "Ubuntu 18.04 LTS", Description = "Ubuntu 18.04 Long Term Support", OsType = OperatingSystemType.Ubuntu1804LTS, Version = "18.04", IsActive = true },
                new VmOperatingSystem { Id = 4, Name = "Ubuntu 20.04 LTS", Description = "Ubuntu 20.04 Long Term Support", OsType = OperatingSystemType.Ubuntu2004LTS, Version = "20.04", IsActive = true },
                new VmOperatingSystem { Id = 5, Name = "Ubuntu 22.04 LTS", Description = "Ubuntu 22.04 Long Term Support", OsType = OperatingSystemType.Ubuntu2204LTS, Version = "22.04", IsActive = true },
                new VmOperatingSystem { Id = 6, Name = "CentOS 7", Description = "CentOS Linux 7", OsType = OperatingSystemType.CentOS7, Version = "7", IsActive = true },
                new VmOperatingSystem { Id = 7, Name = "CentOS 8", Description = "CentOS Linux 8", OsType = OperatingSystemType.CentOS8, Version = "8", IsActive = true },
                new VmOperatingSystem { Id = 8, Name = "Debian 10", Description = "Debian 10 (Buster)", OsType = OperatingSystemType.Debian10, Version = "10", IsActive = true },
                new VmOperatingSystem { Id = 9, Name = "Debian 11", Description = "Debian 11 (Bullseye)", OsType = OperatingSystemType.Debian11, Version = "11", IsActive = true }
            );
            
            // Seed some initial virtual machines
            modelBuilder.Entity<VirtualMachine>().HasData(
                new VirtualMachine
                {
                    Id = 1,
                    Name = "Web Server",
                    Cores = 4,
                    Ram = 16,
                    Disk = 500,
                    OperatingSystemId = 5, // Ubuntu 22.04 LTS
                    StatusId = 1, // Running
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = "Main web server",
                    Hostname = "web-srv-01",
                    IpAddress = "10.0.0.10"
                },
                new VirtualMachine
                {
                    Id = 2,
                    Name = "Database Server",
                    Cores = 8,
                    Ram = 32,
                    Disk = 1000,
                    OperatingSystemId = 2, // Windows Server 2022
                    StatusId = 1, // Running
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = "Main database server",
                    Hostname = "db-srv-01",
                    IpAddress = "10.0.0.11"
                },
                new VirtualMachine
                {
                    Id = 3,
                    Name = "Test Environment",
                    Cores = 2,
                    Ram = 8,
                    Disk = 250,
                    OperatingSystemId = 7, // CentOS 8
                    StatusId = 2, // Stopped
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Description = "Testing environment",
                    Hostname = "test-srv-01",
                    IpAddress = "10.0.0.12"
                }
            );
        }
    }
} 