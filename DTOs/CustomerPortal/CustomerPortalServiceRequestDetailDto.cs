namespace MbaCrm.Api.DTOs
{
    public class CustomerPortalServiceRequestDetailDto
    {
        public int Id { get; set; }

        public string ServiceType { get; set; }
            = string.Empty;

        public string Status { get; set; }
            = string.Empty;

        public string Title { get; set; }
            = string.Empty;

        public string Description { get; set; }
            = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}