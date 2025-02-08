using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AeroBites.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do Item é obrigatório.")]
        [Display(Name = "Nome do Item")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O preço do Item é obrigatório.")]
        [Display(Name = "Preço do Item")]
        public required float Price { get; set; }

        public required int RestaurantId { get; set; }

        [JsonIgnore]
        public Restaurant? Restaurant { get; set; }
    }
}
