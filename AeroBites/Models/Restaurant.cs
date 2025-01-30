using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do Restaurante é obrigatório.")]
        [Display(Name = "Nome do Restaurante")]
        public required string Name { get; set; }

        [DefaultValue(Enums.RestaurantStatus.WaitingAcceptance)]
        public required Enums.RestaurantStatus Status { get; set; }

        public required List<Item> Items { get; set; }

        public required Account Owner { get; set; }
    }
}
