using System.ComponentModel;

namespace AeroBites.Models
{
    public class DropPoint
    {
        public int Id { get; set; }
        public required float Latitude { get; set; }
        public required float Longitude { get; set; }

        public required List<Account> FavoritedBy { get; set; }
    }
}
