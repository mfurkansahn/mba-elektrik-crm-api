namespace MbaCrm.Api.DTOs
{
    public class CustomerAccountListDto
    {
        public string UserId { get; set; }
            = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
            = string.Empty;

        public string FullName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}