using MbaCrm.Api.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MbaCrm.Api.Entities;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Constants;

using Microsoft.AspNetCore.Authorization;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var email = registerDto.Email.Trim();

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return ApiProblem(
                    StatusCodes.Status409Conflict,
                    "Kayıt çakışması.",
                    "Bu e-posta adresi zaten kullanılıyor."
                );
            }

            var user = new ApplicationUser
            {
                FullName = registerDto.FullName.Trim(),
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(
                user,
                registerDto.Password
            );

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(error => new
                    {
                        error.Code,
                        error.Description
                    })
                    .ToList();

                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Kullanıcı oluşturulamadı.",
                    "Personel hesabı oluşturulurken doğrulama hataları oluştu.",
                    errors
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                AppRoles.User
            );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

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
                    "Personel hesabına kullanıcı rolü atanırken hatalar oluştu.",
                    errors
                );
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "Kullanıcı başarıyla oluşturuldu.",
                    userId = user.Id,
                    fullName = user.FullName,
                    email = user.Email
                }
            );
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var email = loginDto.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return ApiProblem(
                    StatusCodes.Status401Unauthorized,
                    "Giriş başarısız.",
                    "E-posta veya şifre hatalı ya da hesap geçici olarak kullanılamıyor."
                );
            }

            var signInResult =
                await _signInManager.CheckPasswordSignInAsync(
                    user,
                    loginDto.Password,
                    lockoutOnFailure: true
                );

            if (signInResult.IsLockedOut)
            {
                return ApiProblem(
                    StatusCodes.Status401Unauthorized,
                    "Giriş başarısız.",
                    "E-posta veya şifre hatalı ya da hesap geçici olarak kullanılamıyor."
                );
            }

            if (!signInResult.Succeeded)
            {
                return ApiProblem(
                    StatusCodes.Status401Unauthorized,
                    "Giriş başarısız.",
                    "E-posta veya şifre hatalı ya da hesap geçici olarak kullanılamıyor."
                );
            }

            var token = await _tokenService.CreateTokenAsync(user);

            var response = new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty
            };

            return Ok(response);
        }
    }
}