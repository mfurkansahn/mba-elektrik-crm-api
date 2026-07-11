using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateCustomerDto
    {
        [Required(
            ErrorMessage = "Müşteri adı veya şirket adı zorunludur."
        )]
        [StringLength(
            200,
            MinimumLength = 2,
            ErrorMessage = "Müşteri adı veya şirket adı 2 ile 200 karakter arasında olmalıdır."
        )]
        public string FullNameOrCompanyName { get; set; } =
            string.Empty;


        [Required(
            ErrorMessage = "Telefon numarası zorunludur."
        )]
        [StringLength(
            20,
            MinimumLength = 10,
            ErrorMessage = "Telefon numarası 10 ile 20 karakter arasında olmalıdır."
        )]
        [RegularExpression(
            @"^[0-9+\-\s()]+$",
            ErrorMessage = "Telefon numarası yalnızca rakam, boşluk, +, -, ( ve ) karakterlerini içerebilir."
        )]
        public string Phone { get; set; } = string.Empty;


        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz."
        )]
        [StringLength(
            200,
            ErrorMessage = "E-posta adresi en fazla 200 karakter olabilir."
        )]
        public string? Email { get; set; }


        [StringLength(
            500,
            ErrorMessage = "Adres en fazla 500 karakter olabilir."
        )]
        public string? Address { get; set; }


        [Required(
            ErrorMessage = "Şehir bilgisi zorunludur."
        )]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Şehir bilgisi 2 ile 100 karakter arasında olmalıdır."
        )]
        public string City { get; set; } = "Ankara";


        [StringLength(
            100,
            ErrorMessage = "İlçe bilgisi en fazla 100 karakter olabilir."
        )]
        public string? District { get; set; }


        [Required(
            ErrorMessage = "Müşteri tipi zorunludur."
        )]
        [RegularExpression(
            "^(Bireysel|Kurumsal)$",
            ErrorMessage = "Müşteri tipi yalnızca 'Bireysel' veya 'Kurumsal' olabilir."
        )]
        public string CustomerType { get; set; } =
            string.Empty;


        [StringLength(
            1000,
            ErrorMessage = "Açıklama en fazla 1000 karakter olabilir."
        )]
        public string? Description { get; set; }
    }
}