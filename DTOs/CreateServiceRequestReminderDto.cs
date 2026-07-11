namespace MbaCrm.Api.DTOs
{
    public class CreateServiceRequestReminderDto
    {
        public string ReminderText { get; set; } = string.Empty;

        public DateTime ReminderDate { get; set; }
    }
}
