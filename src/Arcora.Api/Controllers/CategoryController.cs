// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arcora.Api.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // GET: api/<CategoryController>
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(Name = "GetAllCategories")]
        public async Task<IActionResult> Get([FromServices] ICategoryService categoryService, [FromQuery] Paging paging)
        {
            return Ok(await categoryService.GetAll(paging));
        }

        // GET api/<CategoryController>/5
        [Authorize(Roles = "Viewer, User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetCategoryByID")]
        public async Task<IActionResult> GetCategoryByID([FromServices] ICategoryService categoryService, int id)
        {
            var result = await categoryService.GetID(id);
            if (result == null)
                return NotFound(new { message = "Category with the specified ID was not found." });
            return Ok(result);
        }

        // POST api/<CategoryController>
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromServices] ICategoryService categoryService, [FromBody] CategoryDto categoryDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await categoryService.CreateCategory(categoryDto);
            if (result != null && result.CategoryID != 0)
                return CreatedAtRoute("GetCategoryByID", new { id = result.CategoryID }, result);
            return BadRequest(new { message = "Failed to create category. A category with the same name may already exist." });
        }

        // PUT api/<CategoryController>/5
        [Authorize(Roles = "User, LandLord, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}", Name = "UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromServices] ICategoryService categoryService, int id, [FromBody] CategoryDto categoryDto)
        {
            // var displayName = User.Identity?.Name ?? string.Empty;
            // var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var result = await categoryService.UpdateCategory(id, categoryDto);
            if (result == null)
                return NotFound(new { message = "Category with the specified ID was not found." });
            return Ok(result);
        }

        // DELETE api/<CategoryController>/5
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}", Name = "DeleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromServices] ICategoryService categoryService, int id)
        {
            try
            {
                await categoryService.DeleteCategory(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Category with the specified ID was not found." });
            }
        }
    }
}