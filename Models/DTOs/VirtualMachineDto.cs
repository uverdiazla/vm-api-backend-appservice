using System.ComponentModel.DataAnnotations;

namespace vm_api_backend_appservice.Models.DTOs
{
    public class CreateVirtualMachineDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 128)]
        public int Cores { get; set; }
        
        [Required]
        [Range(1, 1024)]
        public int Ram { get; set; }
        
        [Required]
        [Range(1, 10000)]
        public int Disk { get; set; }
        
        [Required]
        public int OperatingSystemId { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        
        public string? Description { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }
    }
    
    public class UpdateVirtualMachineDto
    {
        [Required]
        [Range(1, 128)]
        public int Cores { get; set; }
        
        [Required]
        [Range(1, 1024)]
        public int Ram { get; set; }
        
        [Required]
        [Range(1, 10000)]
        public int Disk { get; set; }
        
        [Required]
        public int OperatingSystemId { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        
        public string? Description { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }
    }
    
    public class VirtualMachineResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Cores { get; set; }
        public int Ram { get; set; }
        public int Disk { get; set; }
        public OperatingSystemDto OperatingSystem { get; set; } = new();
        public StatusDto Status { get; set; } = new();
        public string? Description { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class OperatingSystemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
    
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
    }
} 