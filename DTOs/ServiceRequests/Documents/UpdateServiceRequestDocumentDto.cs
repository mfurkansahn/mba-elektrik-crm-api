using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestDocumentDto
    {
        [Required(ErrorMessage = "Evrak adı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Evrak adı en fazla 200 karakter olabilir.")]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Description { get; set; }
    }
}