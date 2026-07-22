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
    [Authorize(Roles = AppRoles.Customer)]
    public class CustomerPortalController : ApiControllerBase
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerPortalController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser is null)
            {
                return Unauthorized(
                    "Giriş yapan kullanıcı bulunamadı."
                );
            }

            if (!currentUser.CustomerId.HasValue)
            {
                return Forbid();
            }

            var profile = await _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.Id == currentUser.CustomerId.Value
                )
                .Select(customer =>
                    new CustomerPortalProfileDto
                    {
                        UserId = currentUser.Id,
                        CustomerId = customer.Id,
                        FullName =
                            customer.FullNameOrCompanyName,
                        Email =
                            currentUser.Email ?? string.Empty,
                        Phone =
                            customer.Phone ?? string.Empty,
                        Address =
                            customer.Address ?? string.Empty,
                        City =
                            customer.City ?? string.Empty,
                        District =
                            customer.District ?? string.Empty,
                        CustomerType =
                            customer.CustomerType ?? string.Empty
                    }
                )
                .FirstOrDefaultAsync();

            if (profile is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Bağlı müşteri kaydı bulunamadı."
                );
            }

            return Ok(profile);
        }

        [HttpGet("service-requests")]
        public async Task<IActionResult> GetMyServiceRequests()
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser is null)
            {
                return Unauthorized(
                    "Giriş yapan kullanıcı bulunamadı."
                );
            }

            if (!currentUser.CustomerId.HasValue)
            {
                return Forbid();
            }

            var serviceRequests =
                await _context.ServiceRequests
                    .AsNoTracking()
                    .Where(request =>
                        request.CustomerId ==
                        currentUser.CustomerId.Value
                    )
                    .OrderByDescending(request =>
                        request.CreatedAt
                    )
                    .Select(request =>
                        new CustomerPortalServiceRequestListDto
                        {
                            Id = request.Id,
                            ServiceType =
                                request.ServiceType,
                            Status =
                                request.Status,
                            Title =
                                request.Title,
                            StartDate =
                                request.StartDate,
                            DueDate =
                                request.DueDate,
                            CompletedDate =
                                request.CompletedDate,
                            CreatedAt =
                                request.CreatedAt
                        }
                    )
                    .ToListAsync();

            return Ok(serviceRequests);
        }

        [HttpGet("service-requests/{id:int}")]
        public async Task<IActionResult> GetMyServiceRequestById(
    int id)
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser is null)
            {
                return Unauthorized(
                    "Giriş yapan kullanıcı bulunamadı."
                );
            }

            if (!currentUser.CustomerId.HasValue)
            {
                return Forbid();
            }

            var serviceRequest =
                await _context.ServiceRequests
                    .AsNoTracking()
                    .Where(request =>
                        request.Id == id
                        &&
                        request.CustomerId ==
                        currentUser.CustomerId.Value
                    )
                    .Select(request =>
                        new CustomerPortalServiceRequestDetailDto
                        {
                            Id = request.Id,
                            ServiceType =
                                request.ServiceType,
                            Status =
                                request.Status,
                            Title =
                                request.Title,
                            Description =
                                request.Description ?? string.Empty,
                            StartDate =
                                request.StartDate,
                            DueDate =
                                request.DueDate,
                            CompletedDate =
                                request.CompletedDate,
                            CreatedAt =
                                request.CreatedAt
                        }
                    )
                    .FirstOrDefaultAsync();

            if (serviceRequest is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Hizmet talebi bulunamadı."
                );
            }

            return Ok(serviceRequest);
        }
    }
}