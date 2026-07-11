namespace MbaCrm.Api.Entities
{
    public class ServiceRequestNote
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest ServiceRequest { get; set; } = null!;

        public string NoteText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
