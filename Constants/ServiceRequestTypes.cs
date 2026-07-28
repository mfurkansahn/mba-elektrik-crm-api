namespace MbaCrm.Api.Constants
{
    public static class ServiceRequestTypes
    {
        public const string InteriorInstallationProject =
            "İç Tesisat Projesi";

        public const string AgriculturalIrrigationElectricity =
            "Tarımsal Sulama Elektriği";

        public const string ConstructionSiteElectricity =
            "Şantiye Elektriği";

        public const string SubscriptionProcedures =
            "Abonelik İşlemleri";

        public const string PowerChange =
            "Güç Değişikliği";

        public const string ElectricVehicleChargingStation =
            "Elektrikli Araç Şarj İstasyonu";

        public const string EnerjisaApplicationTracking =
            "Enerjisa Başvuru Takibi";

        public const string HighVoltageOperationResponsibility =
            "Yüksek Gerilim İşletme Sorumluluğu";

        public const string ElectricalProjectConsultancy =
            "Elektrik Proje Danışmanlığı";

        public const string ProjectModificationAndRevision =
            "Proje Tadilatı ve Revizyonu";

        public const string NewElectricalConnectionProcedures =
            "Yeni Elektrik Bağlantı İşlemleri";

        public const string EnergyPermissionProcedures =
            "Enerji Müsaadesi İşlemleri";

        public const string MeterAndMeasurementSystemProcedures =
            "Sayaç ve Ölçüm Sistemi İşlemleri";

        public const string TransformerAndMediumVoltageDesign =
            "Trafo ve Orta Gerilim Projelendirme";

        public const string PowerFactorCorrectionSystem =
            "Kompanzasyon Sistemi";

        public const string GroundingAndLightningProtectionSystems =
            "Topraklama ve Paratoner Sistemleri";

        public const string GeneratorAndUpsSystems =
            "Jeneratör ve UPS Sistemleri";

        public const string SolarEnergySystemProject =
            "Güneş Enerjisi Sistemi Projesi";

        public const string ElectricalInstallationInspectionAndReporting =
            "Elektrik Tesisatı Kontrol ve Raporlama";

        public const string PeriodicElectricalInspection =
            "Periyodik Elektrik Kontrolü";

        public const string EnergyEfficiencyConsultancy =
            "Enerji Verimliliği Danışmanlığı";

        public const string LowCurrentSystems =
            "Zayıf Akım Sistemleri";

        public const string FireDetectionSystems =
            "Yangın Algılama Sistemleri";

        public const string BuildingAutomationAndSmartHomeSystems =
            "Bina Otomasyonu ve Akıllı Ev Sistemleri";

        public const string ElectricalMaintenanceAndFaultServices =
            "Elektrik Bakım ve Arıza Hizmetleri";

        public const string TemporaryAcceptanceProcedures =
            "Geçici Kabul ve Kabul İşlemleri";

        public const string Other =
            "Diğer";

        public static IReadOnlySet<string> All { get; } =
            new HashSet<string>(StringComparer.Ordinal)
            {
                InteriorInstallationProject,
                AgriculturalIrrigationElectricity,
                ConstructionSiteElectricity,
                SubscriptionProcedures,
                PowerChange,
                ElectricVehicleChargingStation,
                EnerjisaApplicationTracking,
                HighVoltageOperationResponsibility,
                ElectricalProjectConsultancy,
                ProjectModificationAndRevision,
                NewElectricalConnectionProcedures,
                EnergyPermissionProcedures,
                MeterAndMeasurementSystemProcedures,
                TransformerAndMediumVoltageDesign,
                PowerFactorCorrectionSystem,
                GroundingAndLightningProtectionSystems,
                GeneratorAndUpsSystems,
                SolarEnergySystemProject,
                ElectricalInstallationInspectionAndReporting,
                PeriodicElectricalInspection,
                EnergyEfficiencyConsultancy,
                LowCurrentSystems,
                FireDetectionSystems,
                BuildingAutomationAndSmartHomeSystems,
                ElectricalMaintenanceAndFaultServices,
                TemporaryAcceptanceProcedures,
                Other
            };

        public static bool IsValid(string serviceType)
        {
            return All.Contains(serviceType);
        }
    }
}