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

        [HttpGet("getAllCategories")]
        public async Task<ActionResult<FetchCategoriesResponse>> GetAllCategories()
        {
            try
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
            catch (Exception)
            {
                return StatusCode(500, new FetchCategoriesResponse
                {
                    Success = false,
                    Message = "Couldn't fetch categories at this moment! Please try again later."
                });
            }
        }

        [HttpGet("getCategory/{categoryId}")]
        public async Task<ActionResult<FetchCategoryResponse>> GetCategory(int CategoryId)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new FetchCategoryResponse
                {
                    Success = false,
                    Message = "Couldn't fetch categories you requested at this moment! Please try again later.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("AddNewCategory")]
        public async Task<ActionResult<AddNewCategoryResponse>> AddNewCategory(AddNewCategoryRequest addNewCategoryRequest)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new AddNewCategoryResponse
                {
                    Success = false,
                    Message = "Couldn't add category you requested at this moment! Please try again later.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("updateCategory/{CategoryId}")]
        public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory(int CategoryId, UpdateCategoryRequest updateCategoryRequest)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new UpdateCategoryResponse
                {
                    Success = false,
                    Message = "Couldn't update category at this moment! Please try again later.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
