using Blog.Data;
using Blog.Extension;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context, [FromServices] IMemoryCache cache)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });
                    await context.Categories.ToListAsync();

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05X04 - Internal Server Failure"));
            }
        }
        private List<Category> GetCategories(BlogDataContext conn)
        {
            return conn.Categories.ToList();
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetAsyncById([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception)
            {
                throw;

            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel categoryViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

                var obj = new Category
                {
                    Id = 0,
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower()
                };

                await context.Categories.AddAsync(obj);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{obj.Id}", new ResultViewModel<Category>(obj));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<string>("Internal database error."));
            }
            catch
            {
                return StatusCode(401, new ResultViewModel<string>("Error"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context, [FromRoute] int id, EditorCategoryViewModel categoryModel)
        {
            try
            {
                var objCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (objCategory == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo nao encontrado"));

                objCategory.Name = categoryModel.Name;
                objCategory.Slug = categoryModel.Slug;

                context.Categories.Update(objCategory);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(objCategory));

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> Delete([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var objCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (objCategory == null)
                    return NotFound(new ResultViewModel<Category>("Object is null"));

                context.Categories.Remove(objCategory);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(objCategory));
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
