using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class EditorCategoryViewModel
    {

        [Required(ErrorMessage = $"Campo nname é obrigatório")]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required(ErrorMessage = "obrigatório esse campo")]
        [MaxLength(50, ErrorMessage = "máximo estrapolado")]
        public string Slug { get; set; }

    }
}
