using MbaCrm.Api.Constants;
using MbaCrm.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User)]

    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalCustomers = await _context.Customers.CountAsync();

            var totalServiceRequests = await _context.ServiceRequests.CountAsync();

            var newRequests = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Yeni Talep");

            var waitingDocuments = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Evrak Bekleniyor");

            var preparingApplications = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Başvuru Hazırlanıyor");

            var enerjisaApplications = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Enerjisa Başvurusu Yapıldı");

            var waitingControl = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Kontrol Bekleniyor");

            var completedRequests = await _context.ServiceRequests
                .CountAsync(x => x.Status == "Tamamlandı");

            var cancelledRequests = await _context.ServiceRequests
                .CountAsync(x => x.Status == "İptal Edildi");

            var result = new
            {
                totalCustomers,
                totalServiceRequests,
                newRequests,
                waitingDocuments,
                preparingApplications,
                enerjisaApplications,
                waitingControl,
                completedRequests,
                cancelledRequests
            };

            return Ok(result);
        }

        [HttpGet("reminders")]
        public async Task<IActionResult> GetReminders()
        {
            var turkeyTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

            var turkeyNow =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    turkeyTimeZone
                );

            var todayStartTurkey = turkeyNow.Date;
            var tomorrowStartTurkey = todayStartTurkey.AddDays(1);
            var upcomingEndTurkey = todayStartTurkey.AddDays(8);

            var todayStartUtc =
                TimeZoneInfo.ConvertTimeToUtc(
                    todayStartTurkey,
                    turkeyTimeZone
                );

            var tomorrowStartUtc =
                TimeZoneInfo.ConvertTimeToUtc(
                    tomorrowStartTurkey,
                    turkeyTimeZone
                );

            var upcomingEndUtc =
                TimeZoneInfo.ConvertTimeToUtc(
                    upcomingEndTurkey,
                    turkeyTimeZone
                );

            // Yalnızca tamamlanmamış hatırlatmalar için temel sorgu
            var incompleteRemindersQuery =
                _context.ServiceRequestReminders
                    .Where(x => !x.IsCompleted);

            // Bütün tamamlanmamış hatırlatmalar
            var totalIncompleteReminders =
                await incompleteRemindersQuery.CountAsync();

            // Bugünden önceki tamamlanmamış hatırlatmalar
            var overdueCount =
                await incompleteRemindersQuery.CountAsync(
                    x => x.ReminderDate < todayStartUtc
                );

            // Türkiye saatine göre bugünkü hatırlatmalar
            var todayCount =
                await incompleteRemindersQuery.CountAsync(
                    x => x.ReminderDate >= todayStartUtc &&
                         x.ReminderDate < tomorrowStartUtc
                );

            // Yarından başlayarak önümüzdeki 7 günlük hatırlatmalar
            var upcomingCount =
                await incompleteRemindersQuery.CountAsync(
                    x => x.ReminderDate >= tomorrowStartUtc &&
                         x.ReminderDate < upcomingEndUtc
                );

            var overdueReminders = await incompleteRemindersQuery
                .Where(x => x.ReminderDate < todayStartUtc)
                .OrderBy(x => x.ReminderDate)
                .Select(x => new
                {
                    reminderId = x.Id,
                    reminderText = x.ReminderText,
                    reminderDate = x.ReminderDate,

                    serviceRequestId = x.ServiceRequestId,
                    serviceRequestTitle = x.ServiceRequest.Title,
                    serviceRequestStatus = x.ServiceRequest.Status,

                    customerId = x.ServiceRequest.CustomerId,
                    customerName = x.ServiceRequest.Customer.FullNameOrCompanyName
                })
                .ToListAsync();

            var todayReminders = await incompleteRemindersQuery
                .Where(x =>
                    x.ReminderDate >= todayStartUtc &&
                    x.ReminderDate < tomorrowStartUtc
                )
                .OrderBy(x => x.ReminderDate)
                .Select(x => new
                {
                    reminderId = x.Id,
                    reminderText = x.ReminderText,
                    reminderDate = x.ReminderDate,

                    serviceRequestId = x.ServiceRequestId,
                    serviceRequestTitle = x.ServiceRequest.Title,
                    serviceRequestStatus = x.ServiceRequest.Status,

                    customerId = x.ServiceRequest.CustomerId,
                    customerName = x.ServiceRequest.Customer.FullNameOrCompanyName
                })
                .ToListAsync();

            var upcomingReminders = await incompleteRemindersQuery
                .Where(x =>
                    x.ReminderDate >= tomorrowStartUtc &&
                    x.ReminderDate < upcomingEndUtc
                )
                .OrderBy(x => x.ReminderDate)
                .Select(x => new
                {
                    reminderId = x.Id,
                    reminderText = x.ReminderText,
                    reminderDate = x.ReminderDate,

                    serviceRequestId = x.ServiceRequestId,
                    serviceRequestTitle = x.ServiceRequest.Title,
                    serviceRequestStatus = x.ServiceRequest.Status,

                    customerId = x.ServiceRequest.CustomerId,
                    customerName = x.ServiceRequest.Customer.FullNameOrCompanyName
                })
                .ToListAsync();

            return Ok(new
            {
                totalIncompleteReminders,
                overdueCount,
                todayCount,
                upcomingCount,
                overdueReminders,
                todayReminders,
                upcomingReminders
            });
        }
    }
}
