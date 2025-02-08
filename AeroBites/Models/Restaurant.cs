using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AeroBites.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do Restaurante é obrigatório.")]
        [Display(Name = "Nome do Restaurante")]
        public required string Name { get; set; }

        [DefaultValue(Enums.RestaurantStatus.WaitingAcceptance)]
        public required Enums.RestaurantStatus Status { get; set; }

        [JsonIgnore]
        public List<Item>? Items { get; set; }

        public required int OwnerId { get; set; }

        public Account? Owner { get; set; }
    }
}
