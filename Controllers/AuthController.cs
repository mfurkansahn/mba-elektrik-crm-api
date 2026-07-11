using MbaCrm.Api.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using MbaCrm.Api.Entities;
using MbaCrm.Api.DTOs;

namespace MbaCrm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var email = registerDto.Email.Trim();

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return BadRequest(
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
                    .Select(error => error.Description);

                return BadRequest(errors);
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
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var email = loginDto.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Unauthorized(
                    "E-posta veya şifre hatalı."
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
                return Unauthorized(
                    "Çok fazla başarısız giriş yapıldı. Hesap geçici olarak kilitlendi."
                );
            }

            if (!signInResult.Succeeded)
            {
                return Unauthorized(
                    "E-posta veya şifre hatalı."
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