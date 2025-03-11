using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class AccountLog
    {
        [Key]
        public int Id { get; set; }

        public required DateTime SignInDateTime { get; set; }

        public required int AccountId { get; set; }
    }
}
