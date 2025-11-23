using expenseTrackerPOC.Data.RequestModels;
using expenseTrackerPOC.Data.ResponseModels;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace expenseTrackerPOC.Controllers.Core
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private IEmailService emailService;
        private IAuthService authService;
        private ICategoryService categoryService;
        private ITransactionService transactionService;

        public CategoryController(IEmailService emailService, IAuthService authService, ICategoryService categoryService, ITransactionService transactionService)
        {
            this.transactionService = transactionService;
            this.emailService = emailService;
            this.authService = authService;
            this.categoryService = categoryService;
        }

        [HttpGet("getAllIcons")]
        public async Task<ActionResult<FetchAllIconsResponse>> GetAllIcons()
        {
            var fetched_Icons = await categoryService.FetchAllIcons();
            if(!fetched_Icons.Success)
            {
                return NotFound(fetched_Icons.icons);
            }
            return Ok(fetched_Icons);
        }

        [HttpGet("getAllCategories")]
        public async Task<ActionResult<FetchCategoriesResponse>> GetAllCategories()
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new FetchCategoriesResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var fetched_categories = await categoryService.FetchAllCategories(userId);
            if(!fetched_categories.Success)
            {
                return NotFound(fetched_categories);
            }
            return Ok(fetched_categories);
        }

        [HttpGet("getCategory/{categoryId}")]
        public async Task<ActionResult<FetchCategoryResponse>> GetCategory(int CategoryId)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new FetchCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var fetched_category = await categoryService.FetchCategoryById(CategoryId, userId);
            if (!fetched_category.Success)
            {
                return NotFound(new FetchCategoryResponse
                {
                    Success = false,
                    Message = "No Category Found."
                });
            }

            return Ok(fetched_category);
        }

        [HttpPost("AddNewCategory")]
        public async Task<ActionResult<AddNewCategoryResponse>> AddNewCategory(AddNewCategoryRequest addNewCategoryRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AddNewCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Input Data",
                    Errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new AddNewCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var added_category = await categoryService.AddNewCategory(addNewCategoryRequest, userId);
            if (!added_category.Success)
            {
                return NotFound(added_category);
            }
            return Ok(added_category);
        }

        [HttpPut("updateCategory/{CategoryId}")]
        public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory(int CategoryId, UpdateCategoryRequest updateCategoryRequest)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var updated_Category = await categoryService.UpdateCategory(CategoryId, updateCategoryRequest, userId);
            if (!updated_Category.Success)
            {
                return NotFound(updated_Category);
            }
            return Ok(updated_Category);
        }

        [HttpDelete("deleteCategory/{CategoryId}")]
        public async Task<ActionResult<DeleteCategoryResponse>> DeleteCategory(int CategoryId)
        {
            var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Id == null)
            {
                return BadRequest(new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "Invalid Request"
                });
            }

            int userId = Convert.ToInt32(Id);

            var deleted_Category = await categoryService.DeleteCategory(CategoryId, userId);
            if (!deleted_Category.Success)
            {
                return NotFound(deleted_Category);
            }
            return Ok(deleted_Category);
        }

    }
}
