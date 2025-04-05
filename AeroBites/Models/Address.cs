using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
        public required double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
        public required double Longitude { get; set; }

        [StringLength(255, ErrorMessage = "O endereço deve ter no máximo 255 caracteres.")]
        public required string FullAddress { get; set; }

        public int RestaurantId { get; set; }
        public int AccountId { get; set; }

        public required bool IsActive { get; set; }
    }
}
