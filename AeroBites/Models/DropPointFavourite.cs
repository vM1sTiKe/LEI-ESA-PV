using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class DropPointFavourite
    {
        [Key]
        public int Id { get; set; }
        public required DropPoint DropPoint { get; set; }

        public required int AccountId { get; set; }
        public required Account Account { get; set; }
    }
}
