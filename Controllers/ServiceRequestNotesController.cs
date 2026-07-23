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
    [Route("api/ServiceRequests/{serviceRequestId}/notes")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User)]
    public class ServiceRequestNotesController : ApiControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestNotesController(AppDbContext context)
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
        public async Task<IActionResult> CreateNote(
            int serviceRequestId,
            CreateServiceRequestNoteDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(sr => sr.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Not eklenmek istenen hizmet talebi bulunamadı."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.NoteText))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz istek.",
                    "Not metni boş olamaz."
                );
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
        public async Task<IActionResult> GetNotesByServiceRequestId(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(sr => sr.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Notları görüntülenmek istenen hizmet talebi bulunamadı."
                );
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
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> DeleteNote(int serviceRequestId, int noteId)
        {
            var note = await _context.ServiceRequestNotes
                .FirstOrDefaultAsync(n =>
                    n.Id == noteId &&
                    n.ServiceRequestId == serviceRequestId);

            if (note is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Silinmek istenen not bulunamadı."
                );
            }

            _context.ServiceRequestNotes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
