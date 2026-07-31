using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.Entities
{
    public class ServiceRequestDocument
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest ServiceRequest { get; set; } = null!;

        [MaxLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        public bool IsDelivered { get; set; } = false;

        public DateTime? DeliveredDate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? OriginalFileName { get; set; }

        [MaxLength(100)]
        public string? StoredFileName { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }

        public long? FileSize { get; set; }

        [MaxLength(500)]
        public string? FilePath { get; set; }

        public DateTime? FileUploadedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}