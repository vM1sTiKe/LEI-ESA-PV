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
        public required Enums.OrderStatus Status { get; set; }

        public required string Restaurant { get; set; }

        public required string Address { get; set; }

        public DateTime? Delivered { get; set; }

        [JsonIgnore]
        public required List<CartItem> Items { get; set; }

        public required int AccountId { get; set; }
    }
}
