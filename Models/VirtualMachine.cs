using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vm_api_backend_appservice.Models
{
    public class VirtualMachine
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 128)]
        public int Cores { get; set; }
        
        [Required]
        [Range(1, 1024)]
        public int Ram { get; set; } // In GB
        
        [Required]
        [Range(1, 10000)]
        public int Disk { get; set; } // In GB
        
        // Foreign keys
        [Required]
        public int OperatingSystemId { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        
        // Navigation properties
        [ForeignKey("OperatingSystemId")]
        public VmOperatingSystem? OperatingSystem { get; set; }
        
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Additional properties that could be added in the future
        public string? Description { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }
    }
} 