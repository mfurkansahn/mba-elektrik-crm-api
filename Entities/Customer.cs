using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string FullNameOrCompanyName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; } = "Ankara";

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(20)]
        public string CustomerType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ServiceRequest> ServiceRequests { get; set; } = new();
    }
}