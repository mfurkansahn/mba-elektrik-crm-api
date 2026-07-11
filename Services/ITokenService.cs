using MbaCrm.Api.Entities;

namespace MbaCrm.Api.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}