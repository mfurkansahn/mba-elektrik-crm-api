using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MbaCrm.Api.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}