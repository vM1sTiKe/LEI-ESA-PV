namespace AeroBites.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public required string Details { get; set; }

        public required Account Account { get; set; }
    }
}
