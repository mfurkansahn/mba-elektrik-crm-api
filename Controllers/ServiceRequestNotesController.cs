using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;

namespace MbaCrm.Api.Controllers
{
    [Route("api/ServiceRequests/{serviceRequestId}/notes")]
    [ApiController]
    [Authorize]
    public class ServiceRequestNotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestNotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(
            int serviceRequestId,
            CreateServiceRequestNoteDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(sr => sr.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(dto.NoteText))
            {
                return BadRequest("Not metni boş olamaz.");
            }

            var note = new ServiceRequestNote
            {
                ServiceRequestId = serviceRequestId,
                NoteText = dto.NoteText
            };

            _context.ServiceRequestNotes.Add(note);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                note.Id,
                note.ServiceRequestId,
                note.NoteText,
                note.CreatedAt
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotesByServiceRequestId(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(sr => sr.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            var notes = await _context.ServiceRequestNotes
                .Where(n => n.ServiceRequestId == serviceRequestId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.ServiceRequestId,
                    n.NoteText,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(notes);
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int serviceRequestId, int noteId)
        {
            var note = await _context.ServiceRequestNotes
                .FirstOrDefaultAsync(n =>
                    n.Id == noteId &&
                    n.ServiceRequestId == serviceRequestId);

            if (note == null)
            {
                return NotFound("Not bulunamadı.");
            }

            _context.ServiceRequestNotes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
