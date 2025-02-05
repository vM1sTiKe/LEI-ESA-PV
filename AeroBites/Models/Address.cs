using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        public required float Latitude { get; set; }
        public required float Longitude { get; set; }

        public required int AccountId { get; set; }

        public required Account Account { get; set; }
    }
}
