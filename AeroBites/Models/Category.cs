using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AeroBites.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da Categoria é obrigatória.")]
        [Display(Name = "Nome da Categoria")]
        public required string Name { get; set; }

        public required int RestaurantId { get; set; }

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}
