using E_Commerce_Inventory.Application.DTOs;

namespace E_Commerce_Inventory.Application.Services
{
    public interface IProductService
    {
        Task<ApiResponseDto<ProductDto>> CreateAsync(CreateProductDto createDto);
        Task<ApiResponseDto<PagedResultDto<ProductDto>>> GetAllAsync(ProductQueryDto queryDto);
        Task<ApiResponseDto<ProductDto>> GetByIdAsync(int id);
        Task<ApiResponseDto<ProductDto>> UpdateAsync(int id, UpdateProductDto updateDto);
        Task<ApiResponseDto<bool>> DeleteAsync(int id);
        Task<ApiResponseDto<IEnumerable<ProductDto>>> SearchAsync(string query);
        Task<ApiResponseDto<ProductDto>> CreateWithFileAsync(CreateProductWithFileDto createDto);
    }
}
