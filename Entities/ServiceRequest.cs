using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        [MaxLength(150)]
        public string ServiceType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Status { get; set; } = "Yeni Talep";

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ServiceRequestNote> Notes { get; set; } = new();

        public List<ServiceRequestDocument> Documents { get; set; } = new();

        public List<ServiceRequestReminder> Reminders { get; set; } = new();
    }
}