using E_Commerce_Inventory.Application.DTOs;

namespace E_Commerce_Inventory.Application.Services
{
    public interface IFileService
    {
        Task<ApiResponseDto<string>> UploadImageAsync(IFormFile file, string folder = "products");
        Task<ApiResponseDto<bool>> DeleteImageAsync(string imageUrl);
        Task<ApiResponseDto<string>> ValidateImageUrlAsync(string imageUrl);
        bool IsValidImageUrl(string url);
        bool IsValidImageFile(IFormFile file);
    }
}
