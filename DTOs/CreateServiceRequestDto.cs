namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestDto
    {
        public int CustomerId { get; set; }

        public string ServiceType { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
