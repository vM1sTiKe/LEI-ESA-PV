using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public required string Details { get; set; }

        public required int AccountId { get; set; }

        public required Account Account { get; set; }
    }
}
