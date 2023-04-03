using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required(ErrorMessage = "obrigatório esse campo")]
        [MaxLength(50, ErrorMessage = "capacidade máxima de caracteres estrapolada.")]
        public string Slug { get; set; }
        public IList<Post> Posts { get; set; }
    }
}