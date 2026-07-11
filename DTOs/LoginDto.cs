using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(
            256,
            ErrorMessage = "E-posta en fazla 256 karakter olabilir."
        )]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Şifre en fazla 100 karakter olabilir."
        )]
        public string Password { get; set; } = string.Empty;
    }
}