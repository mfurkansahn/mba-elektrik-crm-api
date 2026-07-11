using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestStatusDto
    {
        [Required(ErrorMessage = "Durum bilgisi zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Durum bilgisi en fazla 100 karakter olabilir."
        )]
        public string Status { get; set; } = string.Empty;
    }
}