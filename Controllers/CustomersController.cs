using MbaCrm.Api.Data;
using MbaCrm.Api.Entities;
using MbaCrm.Api.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using MbaCrm.Api.Constants;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User)]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context; //Controller’ın veritabanına ulaşmasını sağlar.

        public CustomersController(AppDbContext context) //Dependency Injection
        {
            _context = context; //ASP.NET Core, AppDbContext nesnesini otomatik olarak controller’a verir.
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] string? search,
    [FromQuery] string? customerType,
    [FromQuery] string? city,
    [FromQuery] string? district,
    [FromQuery] DateTime? createdFrom,
    [FromQuery] DateTime? createdTo,
    [FromQuery] string sortBy = "createdAt",
    [FromQuery] string sortDirection = "desc",
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(
                    "Sayfa numarası en az 1 olmalıdır."
                );
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

            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(c =>
                    c.FullNameOrCompanyName.Contains(search) ||
                    c.Phone.Contains(search) ||
                    (c.Email != null &&
                     c.Email.Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(customerType))
            {
                customerType = customerType.Trim();

                query = query.Where(c =>
                    c.CustomerType == customerType);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.Trim();

                query = query.Where(c =>
                    c.City == city);
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                district = district.Trim();

                query = query.Where(c =>
                    c.District == district);
            }

            if (createdFrom.HasValue)
            {
                var createdFromInclusive =
                    createdFrom.Value.Date;

                query = query.Where(c =>
                    c.CreatedAt >= createdFromInclusive);
            }

            if (createdTo.HasValue)
            {
                var createdToExclusive =
                    createdTo.Value.Date.AddDays(1);

                query = query.Where(c =>
                    c.CreatedAt < createdToExclusive);
            }

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

            sortBy = sortBy
                .Trim()
                .ToLowerInvariant();

            sortDirection = sortDirection
                .Trim()
                .ToLowerInvariant();

            if (sortDirection != "asc" &&
                sortDirection != "desc")
            {
                return BadRequest(
                    "Sıralama yönü 'asc' veya 'desc' olmalıdır."
                );
            }

            query = sortBy switch
            {
                "createdat" => sortDirection == "asc"
                    ? query.OrderBy(c => c.CreatedAt)
                    : query.OrderByDescending(c => c.CreatedAt),

                "name" => sortDirection == "asc"
                    ? query.OrderBy(c => c.FullNameOrCompanyName)
                    : query.OrderByDescending(
                        c => c.FullNameOrCompanyName),

                "city" => sortDirection == "asc"
                    ? query.OrderBy(c => c.City)
                    : query.OrderByDescending(c => c.City),

                "district" => sortDirection == "asc"
                    ? query.OrderBy(c => c.District)
                    : query.OrderByDescending(c => c.District),

                "customertype" => sortDirection == "asc"
                    ? query.OrderBy(c => c.CustomerType)
                    : query.OrderByDescending(c => c.CustomerType),

                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.FullNameOrCompanyName,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.City,
                    c.District,
                    c.CustomerType,
                    c.Description,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages,
                items = customers
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) //tek müşteriyi getirir.
        {
            var customer = await _context.Customers
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.Id == id); //Id değeri verilen müşteriyi arar.

            if (customer is null)
            {
                return NotFound("Müşteri bulunamadı."); //Müşteri yoksa 404 döner.
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto) //API’ye gelen veri önce DTO’ya doluyor.
        {
            var customer = new Customer //Sonra biz kendimiz Customer entity’si oluşturuyoruz (mapping)
            {
                FullNameOrCompanyName = dto.FullNameOrCompanyName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                City = dto.City,
                District = dto.District,
                CustomerType = dto.CustomerType,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            }; //DTO’dan gelen veriyi Entity’ye aktardık.

            _context.Customers.Add(customer); //Müşteriyi veritabanına ekler.
            await _context.SaveChangesAsync(); //Müşteriyi kaydeder.

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer); //Başarılı ekleme sonrası 201 Created döner.
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerDto dto) //buradaki 1, id parametresine gelir. / Swagger body’den gelen bilgiler de UpdateCustomerDto içine dolar.
        {
            var customer = await _context.Customers.FindAsync(id); //Veritabanında bu Id’ye sahip müşteriyi arar.

            if (customer is null)
            {
                return NotFound("Müşteri bulunamadı."); //Müşteri yoksa 404 Not Found döner.
            }

            customer.FullNameOrCompanyName = dto.FullNameOrCompanyName; //DTO’dan gelen yeni değerleri mevcut müşteri nesnesine aktarıyoruz.
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;     
            customer.City = dto.City;
            customer.District = dto.District;
            customer.CustomerType = dto.CustomerType;
            customer.Description = dto.Description; //mapping

            await _context.SaveChangesAsync(); //Değişiklikleri SQL Server’a kaydeder.

            return NoContent(); //Başarılı güncellemede 204 No Content döner.
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id); //Veritabanında ilgili müşteriyi arar.

            if (customer is null)
            {
                return NotFound("Müşteri bulunamadı."); //Müşteri yoksa 404 Not Found döner.
            }

            _context.Customers.Remove(customer); //Bu müşteri silinmek üzere işaretlenir.
            await _context.SaveChangesAsync(); //Silme işlemi gerçekten SQL Server’a uygulanır.

            return NoContent(); //Başarılı silme sonrası 204 No Content döner.
        }

        [HttpGet("{customerId}/servicerequests")]
        public async Task<IActionResult> GetServiceRequestsByCustomerId(int customerId)
        {
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
            {
                return NotFound("Müşteri bulunamadı.");
            }

            var serviceRequests = await _context.ServiceRequests
                .Where(sr => sr.CustomerId == customerId)
                .Select(sr => new
                {
                    sr.Id,
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

            return Ok(serviceRequests);
        }
    }
}
