
using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;

namespace expenseTrackerPOC.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        public Task<AddNewCategoryResponse> AddNewCategory(AddNewCategoryRequest addNewCategoryRequest, int userId);
        public Task<FetchCategoriesResponse> FetchAllCategories(int userId);
        public Task<FetchCategoryResponse> FetchCategoryById(int categoryId, int userId);
        public Task<UpdateCategoryResponse> UpdateCategory(int categoryId, UpdateCategoryRequest updateCategoryRequest, int userId);
    }
}
