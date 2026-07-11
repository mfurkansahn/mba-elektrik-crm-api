namespace MbaCrm.Api.Entities
{
    public class Customer
    {
        public int Id { get; set; } //Her müşterinin benzersiz numarasıdır.

        public string FullNameOrCompanyName { get; set; } = string.Empty; //Müşteri adı veya firma adını tutar.

        public string Phone { get; set; } = string.Empty; //Müşterinin telefon numarasını tutar.

        public string? Email { get; set; } //E-posta bilgisini tutar. (Boş olabilir.)

        public string? Address { get; set; } //Adres bilgisini tutar. (Boş olabilir.)

        public string City { get; set; } = "Ankara"; //Şehir bilgisini tutar. (Varsayılan 'Ankara')

        public string? District { get; set; } //İlçe bilgisini tutar.

        public string CustomerType { get; set; } = string.Empty; //Müşteri tipini tutar.

        public string? Description { get; set; } //Müşteri hakkında ek açıklama alanıdır.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //Müşterinin sisteme eklendiği tarihi tutar.

        public List<ServiceRequest> ServiceRequests { get; set; } = new(); //Bu müşteriye ait hizmet taleplerini tutar.
    }
}
