using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do Item é obrigatório.")]
        [Display(Name = "Nome do Item")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O preço do Item é obrigatório.")]
        [Display(Name = "Preço do Item")]
        public required float Price { get; set; }

        public required Restaurant Restaurant { get; set; }
    }
}
