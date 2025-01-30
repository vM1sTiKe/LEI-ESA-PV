namespace AeroBites.Models
{
    public class DropPointFavourite
    {
        public int Id { get; set; }
        public required DropPoint DropPoint { get; set; }
        public required Account Account { get; set; }
    }
}
