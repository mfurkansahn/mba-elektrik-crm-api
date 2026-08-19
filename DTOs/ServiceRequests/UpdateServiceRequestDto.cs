using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestDto : IValidatableObject
    {
        [Required(
            ErrorMessage = "Hizmet türü zorunludur."
        )]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Hizmet türü 2 ile 150 karakter arasında olmalıdır."
        )]
        public string ServiceType { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Durum bilgisi zorunludur."
        )]
        [StringLength(
            100,
            ErrorMessage = "Durum bilgisi en fazla 100 karakter olabilir."
        )]
        public string Status { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Hizmet talebi başlığı zorunludur."
        )]
        [StringLength(
            200,
            MinimumLength = 2,
            ErrorMessage = "Başlık 2 ile 200 karakter arasında olmalıdır."
        )]
        public string Title { get; set; } = string.Empty;


        [StringLength(
            2000,
            ErrorMessage = "Açıklama en fazla 2000 karakter olabilir."
        )]
        public string? Description { get; set; }


        [Required(
            ErrorMessage = "Başlangıç tarihi zorunludur."
        )]
        public DateTime? StartDate { get; set; }


        public DateTime? DueDate { get; set; }


        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (StartDate.HasValue &&
                DueDate.HasValue &&
                DueDate.Value < StartDate.Value)
            {
                yield return new ValidationResult(
                    "Bitiş tarihi başlangıç tarihinden önce olamaz.",
                    new[] { nameof(DueDate) }
                );
            }
        }
    }
}