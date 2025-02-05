using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public required string GoogleId { get; set; }

        [DefaultValue(false)]
        public bool IsAdmin { get; set; }
    }
}
