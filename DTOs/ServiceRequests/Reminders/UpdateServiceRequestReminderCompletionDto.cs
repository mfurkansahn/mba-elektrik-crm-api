using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestReminderCompletionDto
    {
        [Required(
            ErrorMessage = "Hatırlatma tamamlanma durumu zorunludur."
        )]
        public bool? IsCompleted { get; set; }
    }
}