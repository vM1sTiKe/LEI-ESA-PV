using System.ComponentModel;

namespace AeroBites.Models
{
    public class Account
    {
        public int Id { get; set; }

        public required string GoogleId { get; set; }

        [DefaultValue(false)]
        public bool IsAdmin { get; set; }
    }
}
