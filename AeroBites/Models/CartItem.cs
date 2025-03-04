using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public required int Quantity { get; set; }

        public required float Price { get; set; }

        public required int CartId { get; set; }

        public required Cart Cart { get; set; }
    }
}
