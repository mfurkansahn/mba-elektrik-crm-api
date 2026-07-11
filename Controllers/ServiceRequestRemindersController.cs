using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Controllers
{
    [Route("api/ServiceRequests/{serviceRequestId:int}/reminders")]
    [ApiController]
    public class ServiceRequestRemindersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestRemindersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminder(
            int serviceRequestId,
            CreateServiceRequestReminderDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(dto.ReminderText))
            {
                return BadRequest("Hatırlatma metni boş olamaz.");
            }

            if (dto.ReminderDate <= DateTime.UtcNow)
            {
                return BadRequest("Hatırlatma tarihi gelecekte olmalıdır.");
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
        public async Task<IActionResult> GetReminders(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
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
        public async Task<IActionResult> UpdateCompletionStatus(
    int serviceRequestId,
    int reminderId,
    UpdateServiceRequestReminderCompletionDto dto)
        {
            var reminder = await _context.ServiceRequestReminders
                .FirstOrDefaultAsync(x =>
                    x.Id == reminderId &&
                    x.ServiceRequestId == serviceRequestId);

            if (reminder == null)
            {
                return NotFound("Hatırlatma bulunamadı.");
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
        public async Task<IActionResult> DeleteReminder(
    int serviceRequestId,
    int reminderId)
        {
            var reminder = await _context.ServiceRequestReminders
                .FirstOrDefaultAsync(x =>
                    x.Id == reminderId &&
                    x.ServiceRequestId == serviceRequestId);

            if (reminder == null)
            {
                return NotFound("Hatırlatma bulunamadı.");
            }

            _context.ServiceRequestReminders.Remove(reminder);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

