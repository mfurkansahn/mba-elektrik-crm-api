using System.ComponentModel.DataAnnotations;

namespace MbaCrm.Api.DTOs
{
    public class UpdateServiceRequestDocumentDeliveryDto
    {
        [Required(ErrorMessage = "Evrak teslim durumu zorunludur.")]
        public bool? IsDelivered { get; set; }
    }
}