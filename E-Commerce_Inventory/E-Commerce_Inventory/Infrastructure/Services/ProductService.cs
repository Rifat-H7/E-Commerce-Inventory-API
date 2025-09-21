using E_Commerce_Inventory.Application.DTOs;
using E_Commerce_Inventory.Application.Services;
using E_Commerce_Inventory.Domain.Entities;
using E_Commerce_Inventory.Domain.Interfaces;

namespace E_Commerce_Inventory.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<ProductDto>> CreateAsync(CreateProductDto createDto)
        {
            try
            {
                var errors = ValidateCreateProduct(createDto);
                if (errors.Any())
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    };
                }

                // Check if category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(createDto.CategoryId);
                if (category == null)
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                var product = new Product
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Price = createDto.Price,
                    Stock = createDto.Stock,
                    ImageUrl = createDto.ImageUrl,
                    CategoryId = createDto.CategoryId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    CategoryName = category.Name,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return new ApiResponseDto<ProductDto>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = productDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the product",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<PagedResultDto<ProductDto>>> GetAllAsync(ProductQueryDto queryDto)
        {
            try
            {
                // Build filter predicate
                var allProducts = await _unitOfWork.Products.GetAllAsync();
                var query = allProducts.AsQueryable();

                if (queryDto.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == queryDto.CategoryId.Value);
                }

                if (queryDto.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= queryDto.MinPrice.Value);
                }

                if (queryDto.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= queryDto.MaxPrice.Value);
                }

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / queryDto.Limit);

                var products = query
                    .Skip((queryDto.Page - 1) * queryDto.Limit)
                    .Take(queryDto.Limit)
                    .ToList();

                var productDtos = new List<ProductDto>();

                foreach (var product in products)
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);

                    productDtos.Add(new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        ImageUrl = product.ImageUrl,
                        CategoryId = product.CategoryId,
                        CategoryName = category?.Name ?? "Unknown",
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt
                    });
                }

                var pagedResult = new PagedResultDto<ProductDto>
                {
                    Data = productDtos,
                    Page = queryDto.Page,
                    Limit = queryDto.Limit,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNextPage = queryDto.Page < totalPages,
                    HasPreviousPage = queryDto.Page > 1
                };

                return new ApiResponseDto<PagedResultDto<ProductDto>>
                {
                    Success = true,
                    Message = "Products retrieved successfully",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PagedResultDto<ProductDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving products",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<ProductDto>> GetByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.Name ?? "Unknown",
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return new ApiResponseDto<ProductDto>
                {
                    Success = true,
                    Message = "Product retrieved successfully",
                    Data = productDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the product",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<ProductDto>> UpdateAsync(int id, UpdateProductDto updateDto)
        {
            try
            {
                var errors = ValidateUpdateProduct(updateDto);
                if (errors.Any())
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = errors
                    };
                }

                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                // Check if category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(updateDto.CategoryId);
                if (category == null)
                {
                    return new ApiResponseDto<ProductDto>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                product.Name = updateDto.Name;
                product.Description = updateDto.Description;
                product.Price = updateDto.Price;
                product.Stock = updateDto.Stock;
                product.ImageUrl = updateDto.ImageUrl;
                product.CategoryId = updateDto.CategoryId;
                product.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    ImageUrl = product.ImageUrl,
                    CategoryId = product.CategoryId,
                    CategoryName = category.Name,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return new ApiResponseDto<ProductDto>
                {
                    Success = true,
                    Message = "Product updated successfully",
                    Data = productDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<ProductDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the product",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponseDto<bool>> DeleteAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = "Product deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the product",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private List<string> ValidateCreateProduct(CreateProductDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product name is required");
            else if (dto.Name.Length > 200)
                errors.Add("Product name must not exceed 200 characters");

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
                errors.Add("Description must not exceed 1000 characters");

            if (dto.Price <= 0)
                errors.Add("Price must be greater than 0");

            if (dto.Stock < 0)
                errors.Add("Stock cannot be negative");

            if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl.Length > 500)
                errors.Add("Image URL must not exceed 500 characters");

            if (dto.CategoryId <= 0)
                errors.Add("Valid category ID is required");

            return errors;
        }

        private List<string> ValidateUpdateProduct(UpdateProductDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Product name is required");
            else if (dto.Name.Length > 200)
                errors.Add("Product name must not exceed 200 characters");

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
                errors.Add("Description must not exceed 1000 characters");

            if (dto.Price <= 0)
                errors.Add("Price must be greater than 0");

            if (dto.Stock < 0)
                errors.Add("Stock cannot be negative");

            if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl.Length > 500)
                errors.Add("Image URL must not exceed 500 characters");

            if (dto.CategoryId <= 0)
                errors.Add("Valid category ID is required");

            return errors;
        }
    }
}
