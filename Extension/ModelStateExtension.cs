using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extension
{
    public static class ModelStateExtension
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var result = new List<string>();

            foreach (var item in modelState.Values)
            {
                result.AddRange(item.Errors.Select(c => c.ErrorMessage));
            }

            return result;
        }
    }
}
