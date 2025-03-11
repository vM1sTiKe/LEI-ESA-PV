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

        public required string Restaurant { get; set; }

        [JsonIgnore]
        public List<CartItem>? Items { get; set; }

        public required int RestaurantId { get; set; }
        public required int AccountId { get; set; }
    }
}
