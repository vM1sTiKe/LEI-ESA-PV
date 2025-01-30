using System.ComponentModel;

namespace AeroBites.Models
{
    public class Order
    {
        public int Id { get; set; }

        [DefaultValue(Enums.OrderStatus.Choosing)]
        public required Enums.OrderStatus Status { get; set; }

        public required string Restaurant { get; set; }
        public required string Address { get; set; }

        public DateTime? Delivered { get; set; }

        public List<OrderItem>? Items { get; set; }
    }
}
