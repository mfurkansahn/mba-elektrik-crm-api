using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;
using MbaCrm.Api.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace MbaCrm.Api.Controllers
{
    [Route("api/ServiceRequests/{serviceRequestId:int}/reminders")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User)]
    public class ServiceRequestRemindersController : ApiControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestRemindersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status400BadRequest
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> CreateReminder(
            int serviceRequestId,
            CreateServiceRequestReminderDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Hatırlatma eklenmek istenen hizmet talebi bulunamadı."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.ReminderText))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz istek.",
                    "Hatırlatma metni boş olamaz."
                );
            }

            if (dto.ReminderDate <= DateTime.UtcNow)
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz tarih bilgisi.",
                    "Hatırlatma tarihi gelecekte olmalıdır."
                );
            }

            var reminder = new ServiceRequestReminder
            {
                ServiceRequestId = serviceRequestId,
                ReminderText = dto.ReminderText.Trim(),
                ReminderDate = dto.ReminderDate,
                IsCompleted = false,
                CompletedDate = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceRequestReminders.Add(reminder);

            await _context.SaveChangesAsync();

            var response = new
            {
                reminder.Id,
                reminder.ServiceRequestId,
                reminder.ReminderText,
                reminder.ReminderDate,
                reminder.IsCompleted,
                reminder.CompletedDate,
                reminder.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(
    typeof(List<object>),
    StatusCodes.Status200OK
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> GetReminders(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Hatırlatmaları görüntülenmek istenen hizmet talebi bulunamadı."
                );
            }

            var reminders = await _context.ServiceRequestReminders
                .Where(x => x.ServiceRequestId == serviceRequestId)
                .OrderBy(x => x.ReminderDate)
                .Select(x => new
                {
                    x.Id,
                    x.ServiceRequestId,
                    x.ReminderText,
                    x.ReminderDate,
                    x.IsCompleted,
                    x.CompletedDate,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(reminders);
        }

        [HttpPatch("{reminderId:int}/completion")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status400BadRequest
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> UpdateCompletionStatus(
    int serviceRequestId,
    int reminderId,
    UpdateServiceRequestReminderCompletionDto dto)
        {
            var reminder = await _context.ServiceRequestReminders
                .FirstOrDefaultAsync(x =>
                    x.Id == reminderId &&
                    x.ServiceRequestId == serviceRequestId);

            if (reminder is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Tamamlanma durumu güncellenmek istenen hatırlatma bulunamadı."
                );
            }

            var isCompleted = dto.IsCompleted.Value;

            reminder.IsCompleted = isCompleted;

            if (isCompleted)
            {
                reminder.CompletedDate = DateTime.UtcNow;
            }
            else
            {
                reminder.CompletedDate = null;
            }

            await _context.SaveChangesAsync();

            var response = new
            {
                reminder.Id,
                reminder.ServiceRequestId,
                reminder.ReminderText,
                reminder.ReminderDate,
                reminder.IsCompleted,
                reminder.CompletedDate,
                reminder.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{reminderId:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> DeleteReminder(
    int serviceRequestId,
    int reminderId)
        {
            var reminder = await _context.ServiceRequestReminders
                .FirstOrDefaultAsync(x =>
                    x.Id == reminderId &&
                    x.ServiceRequestId == serviceRequestId);

            if (reminder is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Silinmek istenen hatırlatma bulunamadı."
                );
            }

            _context.ServiceRequestReminders.Remove(reminder);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

