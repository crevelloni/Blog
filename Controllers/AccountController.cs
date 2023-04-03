using Blog.Data;
using Blog.Extension;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpPost]
        [Route("v1/accounts")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model, [FromServices] BlogDataContext conn, [FromServices]EmailServices emailService)
        {

            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-"),
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await conn.Users.AddAsync(user);
                await conn.SaveChangesAsync();

                emailService.Send(
                    user.Name,
                    user.Email,
                    "Conta ativada com sucesso!",
                    $"Basta acessar através do link <a href=google.com>Foodle<a>" +
                    $"<br>Soluções em tecnologia.<br>" +
                    $"<b>Cyber Security<b>" +
                    $"<br>Sua senha de primeiro acesso é: <b>{password}<b>.<br>");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    message = "Success in creating account."
                }));
            }
            catch(DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado. "));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Internal Server Failure"));
            }
        }


        [HttpPost]
        [Route("v1/accounts/login")]
        public async Task<IActionResult> Login([FromServices] TokenServices tokenServices, [FromBody] LoginViewModel model, [FromServices]BlogDataContext conn)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await conn
                .Users
                .AsNoTracking()
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(c => c.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = tokenServices.Generatetoken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Internal Server Failure"));
            }

        }

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model, [FromServices] BlogDataContext conn)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Internal Server Failure"));
            }

            var user = await conn.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<Category>("User not found"));

            user.Image = $"{Configuration.UrlImage}/{fileName}";

            try
            {
                conn.Users.Update(user);
                await conn.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Internal Server Failure"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso"));

        }

        [HttpDelete("v1/accounts")]
        public async Task<IActionResult> Delete([FromBody] UserViewModel model, [FromServices] BlogDataContext conn)
        {

            var user = await conn.Users.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return NotFound(new ResultViewModel<Category>("User not found"));

            try
            {
                conn.Users.Remove(user);
                await conn.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Internal Server Failure"));
            }

            return Ok(new ResultViewModel<string>("Success in deleting account."));

        }

    }
}
