using MbaCrm.Api.Constants;
using MbaCrm.Api.Data;
using MbaCrm.Api.Entities;
using MbaCrm.Api.DTOs;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class CustomerAccountsController : ApiControllerBase
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CustomerAccountsController> _logger;

        public CustomerAccountsController(
    AppDbContext context,
    UserManager<ApplicationUser> userManager,
    ILogger<CustomerAccountsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customerAccounts =
                await _context.Users
                    .AsNoTracking()
                    .Where(user =>
                        user.CustomerId.HasValue
                    )
                    .OrderBy(user =>
                        user.FullName
                    )
                    .Select(user =>
                        new CustomerAccountListDto
                        {
                            UserId = user.Id,
                            CustomerId =
                                user.CustomerId!.Value,
                            CustomerName =
                                user.Customer != null
                                    ? user.Customer
                                        .FullNameOrCompanyName
                                    : string.Empty,
                            FullName =
                                user.FullName,
                            Email =
                                user.Email ?? string.Empty,
                            CreatedAt =
                                user.CreatedAt
                        }
                    )
                    .ToListAsync();

            return Ok(customerAccounts);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
    CreateCustomerAccountDto dto)
        {
            var customerExists = await _context.Customers
                .AnyAsync(customer =>
                    customer.Id == dto.CustomerId
                );

            if (!customerExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Belirtilen müşteri kaydı bulunamadı."
                );
            }

            var accountAlreadyExists = await _context.Users
                .AnyAsync(user =>
                    user.CustomerId == dto.CustomerId
                );

            if (accountAlreadyExists)
            {
                return ApiProblem(
                    StatusCodes.Status409Conflict,
                    "Kayıt çakışması.",
                    "Bu müşteriye ait bir portal hesabı zaten bulunmaktadır."
                );
            }

            var normalizedEmail = dto.Email.Trim();

            var existingUser = await _userManager
                .FindByEmailAsync(normalizedEmail);

            if (existingUser is not null)
            {
                return ApiProblem(
                    StatusCodes.Status409Conflict,
                    "Kayıt çakışması.",
                    "Bu e-posta adresi başka bir kullanıcı tarafından kullanılmaktadır."
                );
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName.Trim(),
                Email = normalizedEmail,
                UserName = normalizedEmail,
                CustomerId = dto.CustomerId
            };

            var createResult = await _userManager.CreateAsync(
                user,
                dto.Password
            );

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                    .ToList();

                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Kullanıcı oluşturulamadı.",
                    "Portal hesabı oluşturulurken doğrulama hataları oluştu.",
                    errors
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                AppRoles.Customer
            );

            if (!roleResult.Succeeded)
            {

                _logger.LogWarning(
        "Müşteri rolü atanamadı. Oluşturulan hesap geri alınacak. UserId: {UserId}, CustomerId: {CustomerId}, ErrorCodes: {ErrorCodes}, TraceId: {TraceId}",
        user.Id,
        user.CustomerId,
        string.Join(
            ", ",
            roleResult.Errors.Select(error => error.Code)
        ),
        HttpContext.TraceIdentifier
    );

                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    _logger.LogError(
                        "Rol ataması başarısız olduktan sonra kullanıcı hesabı geri alınamadı. UserId: {UserId}, CustomerId: {CustomerId}, ErrorCodes: {ErrorCodes}, TraceId: {TraceId}",
                        user.Id,
                        user.CustomerId,
                        string.Join(
                            ", ",
                            deleteResult.Errors.Select(error => error.Code)
                        ),
                        HttpContext.TraceIdentifier
                    );
                }

                var errors = roleResult.Errors
                    .Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                    .ToList();

                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Rol atanamadı.",
                    "Portal hesabına müşteri rolü atanırken hatalar oluştu.",
                    errors
                );
            }

            _logger.LogInformation(
    "Müşteri portal hesabı oluşturuldu. UserId: {UserId}, CustomerId: {CustomerId}, TraceId: {TraceId}",
    user.Id,
    user.CustomerId,
    HttpContext.TraceIdentifier
);

            return Created(
                $"/api/CustomerAccounts/{user.Id}",
                new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.CustomerId,
                    Role = AppRoles.Customer
                }
            );
        }
    }
}