using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestNoteDto
    {
        [Required(ErrorMessage = "Not metni zorunludur.")]
        [StringLength(
            2000,
            ErrorMessage = "Not metni en fazla 2000 karakter olabilir."
        )]
        public string NoteText { get; set; } = string.Empty;
    }
}