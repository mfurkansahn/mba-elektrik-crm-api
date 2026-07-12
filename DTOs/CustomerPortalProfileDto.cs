namespace MbaCrm.Api.DTOs
{
    public class CustomerPortalProfileDto
    {
        public string UserId { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string CustomerType { get; set; } = string.Empty;
    }
}