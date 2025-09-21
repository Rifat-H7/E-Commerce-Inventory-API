using E_Commerce_Inventory.Application.DTOs;
using E_Commerce_Inventory.Application.Services;
using E_Commerce_Inventory.Domain.Entities;
using E_Commerce_Inventory.Domain.Interfaces;

namespace E_Commerce_Inventory.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<CategoryDto>> CreateAsync(CreateCategoryDto createDto)
        {
            try
            {
                var errors = ValidateCreateCategory(createDto);
                if (errors.Any())
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    };
                }

                // Check if category name already exists
                var existingCategory = await _unitOfWork.Categories.FirstOrDefaultAsync(c =>
                    c.Name.ToLower() == createDto.Name.ToLower());

                if (existingCategory != null)
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Category with this name already exists"
                    };
                }

                var category = new Category
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = 0,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return new ApiResponseDto<CategoryDto>
                {
                    Success = true,
                    Message = "Category created successfully",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<CategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the category",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var categoryDtos = new List<CategoryDto>();

                foreach (var category in categories)
                {
                    var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

                    categoryDtos.Add(new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        ProductCount = productCount,
                        CreatedAt = category.CreatedAt,
                        UpdatedAt = category.UpdatedAt
                    });
                }

                return new ApiResponseDto<IEnumerable<CategoryDto>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully",
                    Data = categoryDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<CategoryDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving categories",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<CategoryDto>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = productCount,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return new ApiResponseDto<CategoryDto>
                {
                    Success = true,
                    Message = "Category retrieved successfully",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<CategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the category",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto updateDto)
        {
            try
            {
                var errors = ValidateUpdateCategory(updateDto);
                if (errors.Any())
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    };
                }

                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                // Check if another category with the same name exists
                var existingCategory = await _unitOfWork.Categories.FirstOrDefaultAsync(c =>
                    c.Name.ToLower() == updateDto.Name.ToLower() && c.Id != id);

                if (existingCategory != null)
                {
                    return new ApiResponseDto<CategoryDto>
                    {
                        Success = false,
                        Message = "Another category with this name already exists"
                    };
                }

                category.Name = updateDto.Name;
                category.Description = updateDto.Description;
                category.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();

                var productCount = await _unitOfWork.Products.CountAsync(p => p.CategoryId == category.Id);

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = productCount,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return new ApiResponseDto<CategoryDto>
                {
                    Success = true,
                    Message = "Category updated successfully",
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<CategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the category",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                // Check if category has linked products
                var hasProducts = await _unitOfWork.Products.ExistsAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Cannot delete category with linked products"
                    };
                }

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = "Category deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the category",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private List<string> ValidateCreateCategory(CreateCategoryDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Category name is required");
            else if (dto.Name.Length > 100)
                errors.Add("Category name must not exceed 100 characters");

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 500)
                errors.Add("Description must not exceed 500 characters");

            return errors;
        }

        private List<string> ValidateUpdateCategory(UpdateCategoryDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Category name is required");
            else if (dto.Name.Length > 100)
                errors.Add("Category name must not exceed 100 characters");

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 500)
                errors.Add("Description must not exceed 500 characters");

            return errors;
        }
    }
}
