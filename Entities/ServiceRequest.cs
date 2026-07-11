using System.Runtime.ConstrainedExecution;

namespace MbaCrm.Api.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; } //Her hizmet talebinin benzersiz numarasıdır.

        public int CustomerId { get; set; } //Bu hizmet talebinin hangi müşteriye ait olduğunu gösterir.

        public Customer Customer { get; set; } = null!; //Bu hizmet talebinin bağlı olduğu müşteri bilgisidir. - Yani CustomerId sayısal bağlantıdır, Customer ise nesne bağlantısıdır.

        public string ServiceType { get; set; } = string.Empty; //Hizmet türünü tutar.

        public string Status { get; set; } = "Yeni Talep"; //Başvurunun mevcut durumunu tutar. (Varsayılan 'Yeni Talep')

        public string Title { get; set; } = string.Empty; //Hizmet talebinin kısa başlığıdır.

        public string? Description { get; set; } //Hizmet talebiyle ilgili detay açıklamasıdır.

        public DateTime StartDate { get; set; } = DateTime.UtcNow; //Hizmet sürecinin başlangıç tarihidir.

        public DateTime? DueDate { get; set; } //İşin hedef / son takip tarihidir.

        public DateTime? CompletedDate { get; set; } //İşin tamamlandığı tarihtir.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; //Bu hizmet talebinin sisteme eklendiği tarihtir.
        
        public List<ServiceRequestNote> Notes { get; set; } = new();

        public List<ServiceRequestDocument> Documents { get; set; } = new();
        public List<ServiceRequestReminder> Reminders { get; set; } = new();
    }
}
