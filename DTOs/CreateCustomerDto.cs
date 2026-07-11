namespace MbaCrm.Api.DTOs
{
    public class CreateCustomerDto
    {
        public string FullNameOrCompanyName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string City { get; set; } = "Ankara";

        public string? District { get; set; }

        public string CustomerType { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
