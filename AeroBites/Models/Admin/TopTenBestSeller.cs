namespace AeroBites.Models.Admin
{
    public class TopTenBestSeller
    {
        public required string Restaurant { get; set; }
        public required string Item { get; set; }
        public required int Sales { get; set; }
    }
}
