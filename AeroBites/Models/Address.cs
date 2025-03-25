using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
        public double Longitude { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "O endereço deve ter no máximo 255 caracteres.")]
        public string FullAddress { get; set; }

        public required int AccountId { get; set; }
    }
}
