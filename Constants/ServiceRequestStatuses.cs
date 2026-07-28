namespace MbaCrm.Api.Constants
{
    public static class ServiceRequestStatuses
    {
        public const string NewRequest = "Yeni Talep";
        public const string WaitingForDocuments = "Evrak Bekleniyor";
        public const string PreparingApplication = "Başvuru Hazırlanıyor";
        public const string EnerjisaApplicationSubmitted =
            "Enerjisa Başvurusu Yapıldı";
        public const string WaitingForInspection = "Kontrol Bekleniyor";
        public const string Completed = "Tamamlandı";
        public const string Cancelled = "İptal Edildi";

        public static IReadOnlySet<string> All { get; } =
            new HashSet<string>(StringComparer.Ordinal)
            {
                NewRequest,
                WaitingForDocuments,
                PreparingApplication,
                EnerjisaApplicationSubmitted,
                WaitingForInspection,
                Completed,
                Cancelled
            };

        public static bool IsValid(string status)
        {
            return All.Contains(status);
        }
    }
}