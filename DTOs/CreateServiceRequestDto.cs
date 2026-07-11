using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestDto : IValidatableObject
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir müşteri ID değeri giriniz."
        )]
        public int CustomerId { get; set; }


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


        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (DueDate.HasValue &&
                DueDate.Value <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Bitiş tarihi gelecekte olmalıdır.",
                    new[] { nameof(DueDate) }
                );
            }
        }
    }
}