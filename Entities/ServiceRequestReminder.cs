using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.Entities
{
    public class ServiceRequestReminder
    {
        public int Id { get; set; }

        public int ServiceRequestId { get; set; }

        public ServiceRequest ServiceRequest { get; set; } = null!;

        [MaxLength(1000)]
        public string ReminderText { get; set; } = string.Empty;

        public DateTime ReminderDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}