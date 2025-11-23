using Azure.Core;
using expenseTrackerPOC.Data;
using expenseTrackerPOC.Data.Dtos;
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace expenseTrackerPOC.Services.Core
{
    public class CategoryService : ICategoryService
    {
        private ExpenseTrackerDbContext dbContext;
        public CategoryService(ExpenseTrackerDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        public async Task<FetchAllIconsResponse> FetchAllIcons()
        {
            var icons = await dbContext.CategoryIcons.ToListAsync();
            if (icons.Any())
            {
                return new FetchAllIconsResponse
                {
                    Success = true,
                    Message = "Icons fetched successfully!",
                    icons = icons
                };
            }
            return new FetchAllIconsResponse
            {
                Success = false,
                Message = "No Icons for now, Try Again later",
            };
        }

        public async Task<AddNewCategoryResponse> AddNewCategory(AddNewCategoryRequest addNewCategoryRequest, int userId)
        {
            var category = new Category
            {
                UserId = userId,
                CategoryName = addNewCategoryRequest.CategoryName,
                IconId = addNewCategoryRequest.IconId
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            // Fetch with Icon
            var saved = await dbContext.Categories
                .Include(x => x.Icon)
                .FirstOrDefaultAsync(x => x.CategoryId == category.CategoryId);

            var dto = new CategoryDto
            {
                CategoryId = saved.CategoryId,
                CategoryName = saved.CategoryName,
                UserId = saved.UserId,
                IconId = saved.IconId,
                IconUrl = saved.Icon?.IconImageUrl,
                IsDefault = saved.IsDefault == 1
            };

            return new AddNewCategoryResponse
            {
                Success = true,
                Message = "Category Added Successfully",
                Category = dto
            };
        }

        public async Task<FetchCategoriesResponse> FetchAllCategories(int userId)
        {
            var categries = await dbContext.Categories.Include(x => x.Icon).Where(x=>x.IsDefault==1 || x.UserId==userId).OrderBy(c => c.CategoryName).ToListAsync();

            if (categries == null || !categries.Any())
            {

                return new FetchCategoriesResponse
                {
                    Success = false,
                    Message = "No Categories for now, Come back later.",
                    categories = null
                };
            }
            
            var dtoList = categries.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                UserId = c.UserId,
                IconId = c.IconId,
                IconUrl = c.Icon?.IconImageUrl,
                IsDefault = c.IsDefault == 1
            }).ToList();

            return new FetchCategoriesResponse
            {
                Success = true,
                Message = "Categories fetched successfully",
                categories = dtoList
            };
        }

        public async Task<FetchCategoryResponse> FetchCategoryById(int categoryId, int userId)
        {
            var category = await dbContext.Categories
                .Include(c => c.Icon)
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    (c.UserId == userId || c.IsDefault == 1)
                );

            if (category == null)
            {
                return new FetchCategoryResponse
                {
                    Success = false,
                    Message = "No such category found.",
                    Category = null
                };
            }

            var dto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId,
                IconId = category.IconId,
                IconUrl = category.Icon?.IconImageUrl,
                IsDefault = category.IsDefault == 1
            };

            return new FetchCategoryResponse
            {
                Success = true,
                Message = "Category fetched successfully",
                Category = dto
            };
        }

        public async Task<UpdateCategoryResponse> UpdateCategory(int categoryId, UpdateCategoryRequest updateCategoryRequest, int userId)
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    (c.UserId == userId || c.IsDefault == 1)
                );

            if (category == null)
            {
                return new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "No category found!"
                };
            }

            if (category.IsDefault == 1)
            {
                return new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "This is a default Category, You don't have access to update this category!",
                };
            }

            category.CategoryName = updateCategoryRequest.CategoryName;
            category.IconId = updateCategoryRequest.IconId;

            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();

            var dto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UserId = category.UserId,
                IconId = category.IconId,
                IconUrl = (await dbContext.CategoryIcons.FindAsync(category.IconId))?.IconImageUrl,
                IsDefault = category.IsDefault == 1
            };

            return new UpdateCategoryResponse
            {
                Success = true,
                Message = "Category updated successfully",
                Category = dto
            };
        }

        public async Task<DeleteCategoryResponse> DeleteCategory(int categoryId, int userId)
        {
            //1. Find if Category exists and user have access to delete it or not.
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    (c.UserId == userId || c.IsDefault == 1)
                );

            var categoryName = category.CategoryName;

            if (category == null)
            {
                return new DeleteCategoryResponse
                {
                    Success = false,
                    Message = "No such Category found."
                };
            }
            if(category.IsDefault == 1)
            {
                return new DeleteCategoryResponse
                {
                    Success = false,
                    Message = "This is a default Category, You don't have access to update this category!",
                };
            }
            //2. Delete Category
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return new DeleteCategoryResponse
            {
                Success = true,
                Message = $"Category : {categoryName} deleted successfully!",
            };
        }
    }
}
