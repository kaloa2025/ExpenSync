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

        public async Task<AddNewCategoryResponse> AddNewCategory(AddNewCategoryRequest addNewCategoryRequest, int userId)
        {
            Category category = new Category
            {
                UserId = userId,
                CategoryName = addNewCategoryRequest.CategoryName,
                IconId = addNewCategoryRequest.IconId
            };

            //1. Add Category
            var categoryAdded = (await dbContext.Categories.AddAsync(category)).Entity;
            await dbContext.SaveChangesAsync();

            if (categoryAdded == null)
            {
                return new AddNewCategoryResponse
                {
                    Success = false,
                    Message = "Couldn't Add New Category, Try again later.",
                    Category = null
                };
            }

            return new AddNewCategoryResponse
            {
                Success = true,
                Message = "Category Added Scuccessfully",
                Category = categoryAdded
            };
        }

        public async Task<FetchCategoriesResponse> FetchAllCategories(int userId)
        {
            var categries = await dbContext.Categories.Where(x=>x.IsDefault==1 || x.UserId==userId).OrderBy(c => c.CategoryName).ToListAsync();

            if (categries == null || !categries.Any())
            {
                return new FetchCategoriesResponse
                {
                    Success = false,
                    Message = "No Categories for now, Come back later.",
                    categories = new List<Category>()
                };
            }
            return new FetchCategoriesResponse
            {
                Success = true,
                Message = "Categories fetched successfully",
                categories = categries
            };
        }

        public async Task<FetchCategoryResponse> FetchCategoryById(int categoryId, int userId)
        {
            
            var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId && (x.UserId == userId || x.IsDefault == 1));

            if (category == null)
            {
                return new FetchCategoryResponse
                {
                    Success = false,
                    Message = "No such Category found, Come back later.",
                    Category = null
                };
            }
            return new FetchCategoryResponse
            {
                Success = true,
                Message = "Category fetched successfully",
                Category = category
            };
        }

        public async Task<UpdateCategoryResponse> UpdateCategory(int categoryId, UpdateCategoryRequest updateCategoryRequest, int userId)
        {
            if (updateCategoryRequest == null)
            {
                return new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Data submitted",
                };
            }

            // 1. Find the existing user
            var categoryExists = await FetchCategoryById(categoryId, userId);

            if (categoryExists.Success == false)
            {
                return new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "No Category Found!",
                };
            }

            var category = categoryExists.Category;

            // 2. Update category details
            category.CategoryName = updateCategoryRequest.CategoryName;
            category.IconId = updateCategoryRequest.IconId;

            // 3. Save changes
            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();

            // 4. Return updated category
            return new UpdateCategoryResponse
            {
                Success = true,
                Message = "Category Updated Successfully",
                category = category
            };
        }
    }
}
