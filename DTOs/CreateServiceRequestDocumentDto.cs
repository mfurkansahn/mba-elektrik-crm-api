namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestDocumentDto
    {
        public string DocumentName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
