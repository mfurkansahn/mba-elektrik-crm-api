using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Ad soyad 2 ile 150 karakter arasında olmalıdır."
        )]
        public string FullName { get; set; } = string.Empty;

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
            MinimumLength = 8,
            ErrorMessage = "Şifre 8 ile 100 karakter arasında olmalıdır."
        )]
        public string Password { get; set; } = string.Empty;
    }
}