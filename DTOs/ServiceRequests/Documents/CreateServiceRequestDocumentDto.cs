using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestDocumentDto
    {
        [Required(ErrorMessage = "Evrak adı zorunludur.")]
        [StringLength(
            200,
            MinimumLength = 2,
            ErrorMessage = "Evrak adı 2 ile 200 karakter arasında olmalıdır."
        )]
        public string DocumentName { get; set; } = string.Empty;


        [StringLength(
            1000,
            ErrorMessage = "Evrak açıklaması en fazla 1000 karakter olabilir."
        )]
        public string? Description { get; set; }
    }
}