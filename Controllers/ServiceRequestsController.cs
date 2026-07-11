using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context; //Veritabanına ulaşmak için kullanacağımız alan.

        private static readonly List<string> AllowedStatuses = new()
        {
            "Yeni Talep",
            "Evrak Bekleniyor",
            "Başvuru Hazırlanıyor",
            "Enerjisa Başvurusu Yapıldı",
            "Kontrol Bekleniyor",
            "Tamamlandı",
            "İptal Edildi"
        };

        public ServiceRequestsController(AppDbContext context)
        {
            _context = context; //ASP.NET Core bize AppDbContext nesnesini otomatik veriyor.
        } //Ben veritabanıyla çalışacağım, bana AppDbContext lazım.

        [HttpGet]
        public async Task<IActionResult> GetServiceRequests(
    [FromQuery] string? search,
    [FromQuery] string? status,
    [FromQuery] string? serviceType,
    [FromQuery] int? customerId,
    [FromQuery] DateTime? createdFrom,
    [FromQuery] DateTime? createdTo,
    [FromQuery] string sortBy = "createdAt",
    [FromQuery] string sortDirection = "desc",
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest("Sayfa numarası en az 1 olmalıdır.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "Sayfa boyutu 1 ile 100 arasında olmalıdır."
                );
            }

            if (createdFrom.HasValue &&
    createdTo.HasValue &&
    createdFrom.Value.Date > createdTo.Value.Date)
            {
                return BadRequest(
                    "Başlangıç tarihi, bitiş tarihinden büyük olamaz."
                );
            }

            var query = _context.ServiceRequests
            .AsNoTracking()
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(sr =>
                    sr.Title.Contains(search) ||
                    sr.ServiceType.Contains(search) ||
                    (sr.Description != null &&
                     sr.Description.Contains(search)) ||
                    sr.Customer.FullNameOrCompanyName.Contains(search)
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();

                query = query.Where(sr => sr.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(serviceType))
            {
                serviceType = serviceType.Trim();

                query = query.Where(sr =>
                    sr.ServiceType == serviceType);
            }

            if (customerId.HasValue)
            {
                query = query.Where(sr =>
                    sr.CustomerId == customerId.Value);
            }

            sortBy = sortBy.Trim().ToLowerInvariant();
            sortDirection = sortDirection.Trim().ToLowerInvariant();

            if (sortDirection != "asc" && sortDirection != "desc")
            {
                return BadRequest(
                    "Sıralama yönü 'asc' veya 'desc' olmalıdır."
                );
            }

            if (createdFrom.HasValue)
            {
                var createdFromInclusive = createdFrom.Value.Date;

                query = query.Where(sr =>
                    sr.CreatedAt >= createdFromInclusive);
            }

            if (createdTo.HasValue)
            {
                var createdToExclusive =
                    createdTo.Value.Date.AddDays(1);

                query = query.Where(sr =>
                    sr.CreatedAt < createdToExclusive);
            }

            // Filtrelerden sonra toplam kaç kayıt kaldığını hesaplar.
            var totalCount = await query.CountAsync();

            // Toplam sayfa sayısını hesaplar.
            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

            query = sortBy switch
            {
                "createdat" => sortDirection == "asc"
                    ? query.OrderBy(sr => sr.CreatedAt)
                    : query.OrderByDescending(sr => sr.CreatedAt),

                "duedate" => sortDirection == "asc"
                    ? query.OrderBy(sr => sr.DueDate)
                    : query.OrderByDescending(sr => sr.DueDate),

                "startdate" => sortDirection == "asc"
                    ? query.OrderBy(sr => sr.StartDate)
                    : query.OrderByDescending(sr => sr.StartDate),

                "title" => sortDirection == "asc"
                    ? query.OrderBy(sr => sr.Title)
                    : query.OrderByDescending(sr => sr.Title),

                "status" => sortDirection == "asc"
                    ? query.OrderBy(sr => sr.Status)
                    : query.OrderByDescending(sr => sr.Status),

                _ => query.OrderByDescending(sr => sr.CreatedAt)
            };

            var serviceRequests = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sr => new
                {
                    sr.Id,
                    sr.CustomerId,
                    CustomerName = sr.Customer.FullNameOrCompanyName,
                    sr.ServiceType,
                    sr.Status,
                    sr.Title,
                    sr.Description,
                    sr.StartDate,
                    sr.DueDate,
                    sr.CompletedDate,
                    sr.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages,
                items = serviceRequests
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(CreateServiceRequestDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);

            if (customer == null)
            {
                return BadRequest("Bu CustomerId ile müşteri bulunamadı.");
            }

            var serviceRequest = new ServiceRequest
            {
                CustomerId = dto.CustomerId,
                ServiceType = dto.ServiceType,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = "Yeni Talep",
                StartDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceRequestById), new { id = serviceRequest.Id }, new
            {
                serviceRequest.Id,
                serviceRequest.CustomerId,
                serviceRequest.ServiceType,
                serviceRequest.Status,
                serviceRequest.Title,
                serviceRequest.Description,
                serviceRequest.StartDate,
                serviceRequest.DueDate,
                serviceRequest.CompletedDate,
                serviceRequest.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequestById(int id)
        {
            var serviceRequest = await _context.ServiceRequests
                .AsNoTracking()
                .Where(sr => sr.Id == id)
                .Select(sr => new
                {
                    sr.Id,
                    sr.CustomerId,
                    sr.ServiceType,
                    sr.Status,
                    sr.Title,
                    sr.Description,
                    sr.StartDate,
                    sr.DueDate,
                    sr.CompletedDate,
                    sr.CreatedAt,

                    Customer = new
                    {
                        sr.Customer.Id,
                        sr.Customer.FullNameOrCompanyName,
                        sr.Customer.Phone,
                        sr.Customer.Email,
                        sr.Customer.Address,
                        sr.Customer.City,
                        sr.Customer.District,
                        sr.Customer.CustomerType
                    },

                    Notes = sr.Notes
                        .OrderByDescending(note => note.CreatedAt)
                        .Select(note => new
                        {
                            note.Id,
                            note.NoteText,
                            note.CreatedAt
                        })
                        .ToList(),

                                        Documents = sr.Documents
                        .OrderBy(document => document.CreatedAt)
                        .Select(document => new
                        {
                            document.Id,
                            document.DocumentName,
                            document.IsDelivered,
                            document.DeliveredDate,
                            document.Description,
                            document.CreatedAt
                        })
                        .ToList(),

                                    Reminders = sr.Reminders
                        .OrderBy(reminder => reminder.ReminderDate)
                        .Select(reminder => new
                        {
                            reminder.Id,
                            reminder.ReminderText,
                            reminder.ReminderDate,
                            reminder.IsCompleted,
                            reminder.CompletedDate,
                            reminder.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (serviceRequest == null)
            {
                return NotFound("Hizmet talebi bulunamadı.");
            }

            return Ok(serviceRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, UpdateServiceRequestDto dto)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            if (!AllowedStatuses.Contains(dto.Status))
            {
                return BadRequest("Geçersiz durum bilgisi.");
            }

            serviceRequest.ServiceType = dto.ServiceType;
            serviceRequest.Status = dto.Status;
            serviceRequest.Title = dto.Title;
            serviceRequest.Description = dto.Description;
            serviceRequest.DueDate = dto.DueDate;
            serviceRequest.CompletedDate = dto.CompletedDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, UpdateServiceRequestStatusDto dto)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            if (!AllowedStatuses.Contains(dto.Status))
            {
                return BadRequest("Geçersiz durum bilgisi.");
            }

            serviceRequest.Status = dto.Status;

            if (dto.Status == "Tamamlandı")
            {
                serviceRequest.CompletedDate = DateTime.UtcNow;
            }
            else
            {
                serviceRequest.CompletedDate = null;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
