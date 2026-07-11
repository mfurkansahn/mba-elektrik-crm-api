namespace MbaCrm.Api.Entities
{
    public class ServiceRequestDocument
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest ServiceRequest { get; set; } = null!;

        public string DocumentName { get; set; } = string.Empty;

        public bool IsDelivered { get; set; } = false;

        public DateTime? DeliveredDate { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
