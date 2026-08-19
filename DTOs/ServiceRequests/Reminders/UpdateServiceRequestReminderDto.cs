using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestReminderDto
    {
        [Required(ErrorMessage = "Hatırlatma metni zorunludur.")]
        [MaxLength(1000, ErrorMessage = "Hatırlatma metni en fazla 1000 karakter olabilir.")]
        public string ReminderText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hatırlatma tarihi zorunludur.")]
        public DateTime ReminderDate { get; set; }
    }
}