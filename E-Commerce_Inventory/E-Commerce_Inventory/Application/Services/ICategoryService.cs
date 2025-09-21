using E_Commerce_Inventory.Application.DTOs;

namespace E_Commerce_Inventory.Application.Services
{
    public interface ICategoryService
    {
        Task<ApiResponseDto<CategoryDto>> CreateAsync(CreateCategoryDto createDto);
        Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllAsync();
        Task<ApiResponseDto<CategoryDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto updateDto);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
    }
}
