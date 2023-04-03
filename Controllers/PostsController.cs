using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BlogDataContext _blogDataContext;

        public PostsController(BlogDataContext blogDataContext)
        {
            _blogDataContext = blogDataContext;
        }

        [HttpGet]
        [Route("v1/posts")]
        public async Task<IActionResult> GetAsync([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var count = _blogDataContext.Posts.AsNoTracking().Count();
                var posts = await _blogDataContext.Posts
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Author)
                    .Select(p => new ListPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        lastUpdateDate = p.LastUpdateDate,
                        Category = p.Category.Name,
                        Author = $"{p.Author.Name} ({p.Author.Email})"

                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(d => d.lastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts

                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("50X04 - Internal Server Failure"));
            }

        }

        [HttpGet]
        [Route("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync([FromRoute] int id)
        {
            try
            {
                var count = _blogDataContext.Posts.AsNoTracking().Count();
                var post = await _blogDataContext.Posts
                    .AsNoTracking()
                    .Include(c => c.Author)
                    .ThenInclude(c => c.Roles)
                    .Include(c => c.Category)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("50X04 - Internal Server Failure"));
            }

        }

        [HttpGet]
        [Route("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync([FromQuery] int page, [FromQuery] int pageSize, [FromRoute] string category)
        {
            try
            {
                var count = await _blogDataContext.Posts.AsNoTracking().CountAsync();
                var posts = await _blogDataContext.Posts
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Author)
                    .Where(x => x.Category.Slug == category)
                    .Select(p => new ListPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        lastUpdateDate = p.LastUpdateDate,
                        Category = p.Category.Name,
                        Author = $"{p.Author.Name} ({p.Author.Email})"

                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(d => d.lastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts

                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("50X04 - Internal Server Failure"));
            }

        }
    }
}
