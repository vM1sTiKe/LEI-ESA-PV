using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class CartAddress
    {
        [Key]
        public int Id { get; set; }

        public required double Latitude { get; set; }

        public required double Longitude { get; set; }

        public required string FullAddress { get; set; }
    }
}
