using Blog.Models;
using System.Security.Claims;

namespace Blog.Extension
{
    public static class RoleClaimExtension
    {
        public static IEnumerable<Claim> GetClaim(this User user)
        {

            var result = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
            };

            result.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug)));

            return result;
        }

    }
}
