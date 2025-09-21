using E_Commerce_Inventory.Application.DTOs;
using E_Commerce_Inventory.Application.Services;

namespace E_Commerce_Inventory.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public FileService(IWebHostEnvironment environment, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return request == null ? "http://localhost" : $"{request.Scheme}://{request.Host}";
        }

        public string GetImageUrl(string folder, string fileName)
        {
            return $"{GetBaseUrl()}/uploads/{folder}/{fileName}";
        }

        public async Task<ApiResponseDto<string>> UploadImageAsync(IFormFile file, string folder = "products")
        {
            try
            {
                if (!IsValidImageFile(file))
                {
                    return new ApiResponseDto<string>
                    {
                        Success = false,
                        Message = "Invalid image file. Allowed formats: JPG, JPEG, PNG, GIF, WEBP. Max size: 5MB."
                    };
                }

                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = GetBaseUrl();
                var imageUrl = GetImageUrl(folder, fileName);


                return new ApiResponseDto<string>
                {
                    Success = true,
                    Message = "Image uploaded successfully",
                    Data = imageUrl
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "An error occurred while uploading the image",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return new ApiResponseDto<bool> { Success = true, Data = true };

                var baseUrl = GetBaseUrl();
                if (!imageUrl.StartsWith(baseUrl))
                    return new ApiResponseDto<bool> { Success = true, Data = true };

                var relativePath = imageUrl.Replace(baseUrl, "").TrimStart('/');
                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                return new ApiResponseDto<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Error deleting image",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<string>> ValidateImageUrlAsync(string imageUrl)
        {
            try
            {
                if (!IsValidImageUrl(imageUrl))
                    return new ApiResponseDto<string> { Success = false, Message = "Invalid image URL format" };

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var request = new HttpRequestMessage(HttpMethod.Head, imageUrl);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new ApiResponseDto<string> { Success = false, Message = "Image URL is not accessible" };

                var contentType = response.Content.Headers.ContentType?.MediaType?.ToLower();
                if (contentType == null || !_allowedContentTypes.Contains(contentType))
                    return new ApiResponseDto<string> { Success = false, Message = "URL does not point to a valid image file" };

                return new ApiResponseDto<string> { Success = true, Data = imageUrl };
            }
            catch
            {
                return new ApiResponseDto<string> { Success = false, Message = "Error validating image URL" };
            }
        }

        public bool IsValidImageUrl(string url)
        {
            return !string.IsNullOrWhiteSpace(url) &&
                   Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0 || file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension) &&
                   _allowedContentTypes.Contains(file.ContentType.ToLowerInvariant());
        }
    }
}
