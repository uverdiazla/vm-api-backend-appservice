using System.ComponentModel.DataAnnotations;
using vm_api_backend_appservice.Models.Enums;

namespace vm_api_backend_appservice.Models
{
    public class VmOperatingSystem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public OperatingSystemType OsType { get; set; }
        
        [StringLength(50)]
        public string? Version { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation property
        public ICollection<VirtualMachine> VirtualMachines { get; set; } = new List<VirtualMachine>();
    }
} 