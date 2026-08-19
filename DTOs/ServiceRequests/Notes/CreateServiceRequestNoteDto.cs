using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestNoteDto
    {
        [Required(ErrorMessage = "Not metni boş olamaz.")]
        [StringLength(
            2000,
            MinimumLength = 1,
            ErrorMessage = "Not metni 1 ile 2000 karakter arasında olmalıdır."
        )]
        public string NoteText { get; set; } = string.Empty;
    }
}