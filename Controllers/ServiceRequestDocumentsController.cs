using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Controllers
{
    [Route("api/ServiceRequests/{serviceRequestId}/documents")]
    [ApiController]
    public class ServiceRequestDocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestDocumentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument(int serviceRequestId, CreateServiceRequestDocumentDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(dto.DocumentName))
            {
                return BadRequest("Evrak adı boş olamaz.");
            }

            var document = new ServiceRequestDocument
            {
                ServiceRequestId = serviceRequestId,
                DocumentName = dto.DocumentName.Trim(),
                Description = dto.Description?.Trim(),
                IsDelivered = false,
                DeliveredDate = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceRequestDocuments.Add(document);

            await _context.SaveChangesAsync();

            var response = new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.IsDelivered,
                document.DeliveredDate,
                document.Description,
                document.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            var documents = await _context.ServiceRequestDocuments
                .Where(x => x.ServiceRequestId == serviceRequestId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.ServiceRequestId,
                    x.DocumentName,
                    x.IsDelivered,
                    x.DeliveredDate,
                    x.Description,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(documents);
        }

        [HttpPatch("{documentId:int}/delivery")]
        public async Task<IActionResult> UpdateDeliveryStatus(
    int serviceRequestId,
    int documentId,
    UpdateServiceRequestDocumentDeliveryDto dto)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document == null)
            {
                return NotFound("Evrak bulunamadı.");
            }

            document.IsDelivered = dto.IsDelivered;

            if (dto.IsDelivered)
            {
                document.DeliveredDate = DateTime.UtcNow;
            }
            else
            {
                document.DeliveredDate = null;
            }

            await _context.SaveChangesAsync();

            var response = new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.IsDelivered,
                document.DeliveredDate,
                document.Description,
                document.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{documentId:int}")]
        public async Task<IActionResult> DeleteDocument(int serviceRequestId, int documentId)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document == null)
            {
                return NotFound("Evrak bulunamadı.");
            }

            _context.ServiceRequestDocuments.Remove(document);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

