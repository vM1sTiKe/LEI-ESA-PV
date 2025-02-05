using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public required string Details { get; set; }

        public int AccountId { get; set; }

        public Account? Account { get; set; }
    }
}
