namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestDto
    {
        public string ServiceType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }
    }
}
