using MbaCrm.Api.Constants;
using MbaCrm.Api.Data;
using MbaCrm.Api.Entities;
using MbaCrm.Api.DTOs;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class CustomerAccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerAccountsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                return NotFound(
                    "Belirtilen müşteri kaydı bulunamadı."
                );
            }

            var accountAlreadyExists = await _context.Users
                .AnyAsync(user =>
                    user.CustomerId == dto.CustomerId
                );

            if (accountAlreadyExists)
            {
                return Conflict(
                    "Bu müşteriye ait bir portal hesabı zaten bulunmaktadır."
                );
            }

            var normalizedEmail = dto.Email.Trim();

            var existingUser = await _userManager
                .FindByEmailAsync(normalizedEmail);

            if (existingUser is not null)
            {
                return Conflict(
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
                return BadRequest(
                    createResult.Errors.Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                AppRoles.Customer
            );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return BadRequest(
                    roleResult.Errors.Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                );
            }

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