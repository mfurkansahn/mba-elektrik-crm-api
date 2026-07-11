using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestReminderDto : IValidatableObject
    {
        [Required(ErrorMessage = "Hatırlatma metni zorunludur.")]
        [StringLength(
            1000,
            MinimumLength = 2,
            ErrorMessage = "Hatırlatma metni 2 ile 1000 karakter arasında olmalıdır."
        )]
        public string ReminderText { get; set; } = string.Empty;


        public DateTime ReminderDate { get; set; }


        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (ReminderDate == default)
            {
                yield return new ValidationResult(
                    "Hatırlatma tarihi zorunludur.",
                    new[] { nameof(ReminderDate) }
                );

                yield break;
            }

            if (ReminderDate <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Hatırlatma tarihi gelecekte olmalıdır.",
                    new[] { nameof(ReminderDate) }
                );
            }
        }
    }
}