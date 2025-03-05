using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class AccountLog
    {
        [Key]
        public int Id { get; set; }

        public required DateTime signInDate { get; set; }
    }
}
