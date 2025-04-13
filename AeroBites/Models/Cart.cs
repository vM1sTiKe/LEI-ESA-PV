using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AeroBites.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [DefaultValue(Enums.OrderStatus.Choosing)]
        public Enums.OrderStatus Status { get; set; }

        public required string RestaurantName { get; set; }

        [JsonIgnore]
        public List<CartItem>? Items { get; set; }

        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        public required int AccountId { get; set; }
        public int? CartAddressId { get; set; }
        public CartAddress? CartAddress { get; set; }

        public DateOnly PlacedDate { get; set; }
        public float TotalPrice { get; set; }
    }
}
