using MbaCrm.Api.Constants;
using MbaCrm.Api.Data;
using MbaCrm.Api.DTOs;
using MbaCrm.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Controllers
{
    [Route("api/ServiceRequests/{serviceRequestId}/documents")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.User)]
    public class ServiceRequestDocumentsController : ApiControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 10 * 1024 * 1024;

        private static readonly Dictionary<string, string> AllowedFileTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".pdf"] = "application/pdf",
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png"
            };

        private static async Task<bool> HasValidFileSignatureAsync(
    IFormFile file,
    string extension)
        {
            byte[] expectedSignature = extension.ToLowerInvariant() switch
            {
                ".pdf" => new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D },
                ".jpg" => new byte[] { 0xFF, 0xD8, 0xFF },
                ".jpeg" => new byte[] { 0xFF, 0xD8, 0xFF },
                ".png" => new byte[]
                {
            0x89, 0x50, 0x4E, 0x47,
            0x0D, 0x0A, 0x1A, 0x0A
                },
                _ => Array.Empty<byte>()
            };

            if (expectedSignature.Length == 0)
            {
                return false;
            }

            var fileHeader = new byte[expectedSignature.Length];

            await using var stream = file.OpenReadStream();

            var bytesRead = await stream.ReadAsync(
                fileHeader.AsMemory(0, fileHeader.Length)
            );

            return bytesRead == expectedSignature.Length &&
                   fileHeader.SequenceEqual(expectedSignature);
        }

        private const long MaxUploadRequestSize =
    MaxFileSize + (64 * 1024);


        public ServiceRequestDocumentsController(
    AppDbContext context,
    IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status400BadRequest
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> CreateDocument(int serviceRequestId, CreateServiceRequestDocumentDto dto)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Evrak eklenmek istenen hizmet talebi bulunamadı."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.DocumentName))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz istek.",
                    "Evrak adı boş olamaz."
                );
            }

            var document = new ServiceRequestDocument
            {
                ServiceRequestId = serviceRequestId,
                DocumentName = dto.DocumentName.Trim(),
                Description = dto.Description?.Trim(),
                IsDelivered = false,
                DeliveredDate = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceRequestDocuments.Add(document);

            await _context.SaveChangesAsync();

            var response = new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.IsDelivered,
                document.DeliveredDate,
                document.Description,
                document.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(
    typeof(List<object>),
    StatusCodes.Status200OK
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> GetDocuments(int serviceRequestId)
        {
            var serviceRequestExists = await _context.ServiceRequests
                .AnyAsync(x => x.Id == serviceRequestId);

            if (!serviceRequestExists)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Evrakları görüntülenmek istenen hizmet talebi bulunamadı."
                );
            }

            var documents = await _context.ServiceRequestDocuments
                .Where(x => x.ServiceRequestId == serviceRequestId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.ServiceRequestId,
                    x.DocumentName,
                    x.IsDelivered,
                    x.DeliveredDate,
                    x.Description,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(documents);
        }

        [HttpPut("{documentId:int}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDocument(
    int serviceRequestId,
    int documentId,
    UpdateServiceRequestDocumentDto dto)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Güncellenmek istenen evrak bulunamadı."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.DocumentName))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz istek.",
                    "Evrak adı boş olamaz."
                );
            }

            document.DocumentName = dto.DocumentName.Trim();
            document.Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.Description,
                document.IsDelivered,
                document.DeliveredDate,
                document.CreatedAt
            });
        }

        [HttpPatch("{documentId:int}/delivery")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status400BadRequest
)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound
)]
        public async Task<IActionResult> UpdateDeliveryStatus(
            int serviceRequestId,
            int documentId,
            UpdateServiceRequestDocumentDeliveryDto dto)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Teslim durumu güncellenmek istenen evrak bulunamadı."
                );
            }

            var isDelivered = dto.IsDelivered!.Value;

            document.IsDelivered = isDelivered;

            if (isDelivered)
            {
                document.DeliveredDate = DateTime.UtcNow;
            }
            else
            {
                document.DeliveredDate = null;
            }

            await _context.SaveChangesAsync();

            var response = new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.IsDelivered,
                document.DeliveredDate,
                document.Description,
                document.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost("{documentId:int}/file")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxUploadRequestSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadDocumentFile(
    int serviceRequestId,
    int documentId,
    IFormFile file)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Dosya yüklenmek istenen evrak bulunamadı."
                );
            }

            if (file is null || file.Length == 0)
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz dosya.",
                    "Yüklenecek dosya boş olamaz."
                );
            }

            if (file.Length > MaxFileSize)
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Dosya çok büyük.",
                    "Dosya boyutu en fazla 10 MB olabilir."
                );
            }

            var originalFileName = Path.GetFileName(file.FileName).Trim();

            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz dosya.",
                    "Dosya adı geçerli değil."
                );
            }

            if (originalFileName.Length > 255)
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz dosya.",
                    "Dosya adı en fazla 255 karakter olabilir."
                );
            }

            var extension = Path.GetExtension(originalFileName);

            if (!AllowedFileTypes.TryGetValue(
                extension,
                out var expectedContentType))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Desteklenmeyen dosya türü.",
                    "Yalnızca PDF, JPG, JPEG ve PNG dosyaları yüklenebilir."
                );
            }

            if (!string.Equals(
                file.ContentType,
                expectedContentType,
                StringComparison.OrdinalIgnoreCase))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz dosya türü.",
                    "Dosyanın uzantısı ile içerik türü uyuşmuyor."
                );
            }

            if (!await HasValidFileSignatureAsync(file, extension))
            {
                return ApiProblem(
                    StatusCodes.Status400BadRequest,
                    "Geçersiz dosya içeriği.",
                    "Dosyanın içeriği, belirtilen dosya türüyle uyuşmuyor."
                );
            }

            var storageDirectory = Path.Combine(
                _environment.ContentRootPath,
                "Storage",
                "ServiceRequestDocuments",
                serviceRequestId.ToString(),
                documentId.ToString()
            );

            Directory.CreateDirectory(storageDirectory);

            var storedFileName =
                $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

            var fullFilePath = Path.Combine(
                storageDirectory,
                storedFileName
            );

            await using (var stream = new FileStream(
                fullFilePath,
                FileMode.CreateNew,
                FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            var previousStoredFileName = document.StoredFileName;

            document.OriginalFileName = originalFileName;
            document.StoredFileName = storedFileName;
            document.ContentType = expectedContentType;
            document.FileSize = file.Length;
            document.FilePath =
                $"Storage/ServiceRequestDocuments/{serviceRequestId}/{documentId}/{storedFileName}";
            document.FileUploadedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                throw;
            }

            if (!string.IsNullOrWhiteSpace(previousStoredFileName))
            {
                var previousFilePath = Path.Combine(
                    storageDirectory,
                    Path.GetFileName(previousStoredFileName)
                );

                if (System.IO.File.Exists(previousFilePath))
                {
                    System.IO.File.Delete(previousFilePath);
                }
            }

            var response = new
            {
                document.Id,
                document.ServiceRequestId,
                document.DocumentName,
                document.OriginalFileName,
                document.ContentType,
                document.FileSize,
                document.FileUploadedAt
            };

            return Ok(response);
        }

        [HttpGet("{documentId:int}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadDocumentFile(
    int serviceRequestId,
    int documentId)
        {
            var document = await _context.ServiceRequestDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Dosyası indirilmek istenen evrak bulunamadı."
                );
            }

            if (string.IsNullOrWhiteSpace(document.StoredFileName))
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Dosya bulunamadı.",
                    "Bu evraka henüz bir dosya yüklenmemiş."
                );
            }

            var fullFilePath = Path.Combine(
                _environment.ContentRootPath,
                "Storage",
                "ServiceRequestDocuments",
                serviceRequestId.ToString(),
                documentId.ToString(),
                Path.GetFileName(document.StoredFileName)
            );

            if (!System.IO.File.Exists(fullFilePath))
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Dosya bulunamadı.",
                    "Dosya depolama alanında bulunamadı."
                );
            }

            var contentType =
                document.ContentType ?? "application/octet-stream";

            var downloadFileName =
                document.OriginalFileName ?? document.StoredFileName;

            return PhysicalFile(
                fullFilePath,
                contentType,
                downloadFileName,
                enableRangeProcessing: true
            );
        }

        [HttpDelete("{documentId:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(
    int serviceRequestId,
    int documentId)
        {
            var document = await _context.ServiceRequestDocuments
                .FirstOrDefaultAsync(x =>
                    x.Id == documentId &&
                    x.ServiceRequestId == serviceRequestId);

            if (document is null)
            {
                return ApiProblem(
                    StatusCodes.Status404NotFound,
                    "Kayıt bulunamadı.",
                    "Silinmek istenen evrak bulunamadı."
                );
            }

            var documentDirectory = Path.Combine(
                _environment.ContentRootPath,
                "Storage",
                "ServiceRequestDocuments",
                serviceRequestId.ToString(),
                documentId.ToString()
            );

            string? fullFilePath = null;

            if (!string.IsNullOrWhiteSpace(document.StoredFileName))
            {
                fullFilePath = Path.Combine(
                    documentDirectory,
                    Path.GetFileName(document.StoredFileName)
                );
            }

            _context.ServiceRequestDocuments.Remove(document);

            // Önce veritabanı kaydı güvenle silinir.
            await _context.SaveChangesAsync();

            // Sonra bağlı fiziksel dosya temizlenir.
            if (fullFilePath is not null &&
                System.IO.File.Exists(fullFilePath))
            {
                System.IO.File.Delete(fullFilePath);
            }

            // Evraka ait klasör boş kaldıysa klasörü de temizler.
            if (Directory.Exists(documentDirectory) &&
                !Directory.EnumerateFileSystemEntries(documentDirectory).Any())
            {
                Directory.Delete(documentDirectory);
            }

            return NoContent();
        }
    }
}

