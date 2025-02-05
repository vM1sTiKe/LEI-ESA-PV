using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AeroBites.Models
{
    public class DropPoint
    {
        [Key]
        public int Id { get; set; }
        public required float Latitude { get; set; }
        public required float Longitude { get; set; }
    }
}
