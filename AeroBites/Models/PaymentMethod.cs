using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }

        [DefaultValue("PayPal")]
        public string? Type { get; set; }

        public required string Details { get; set; }

        public required string ApiToken { get; set; }

        public int RestaurantId { get; set; }
        public int AccountId { get; set; }

        [DefaultValue(false)]
        public bool? IsDefault { get; set; }
    }
}
