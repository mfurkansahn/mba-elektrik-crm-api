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


        public DateTime? DueDate { get; set; }


        public DateTime? CompletedDate { get; set; }


        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (CompletedDate.HasValue &&
                CompletedDate.Value > DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Tamamlanma tarihi gelecekte olamaz.",
                    new[] { nameof(CompletedDate) }
                );
            }

            if (Status == "Tamamlandı" &&
                !CompletedDate.HasValue)
            {
                yield return new ValidationResult(
                    "Tamamlanan hizmet taleplerinde tamamlanma tarihi zorunludur.",
                    new[] { nameof(CompletedDate) }
                );
            }

            if (Status != "Tamamlandı" &&
                CompletedDate.HasValue)
            {
                yield return new ValidationResult(
                    "Tamamlanmamış hizmet taleplerinde tamamlanma tarihi boş olmalıdır.",
                    new[] { nameof(CompletedDate) }
                );
            }
        }
    }
}