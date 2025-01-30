namespace AeroBites.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required int Quantity { get; set; }

        public required float Price { get; set; }

        public required Order Order { get; set; }
    }
}
