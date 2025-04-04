namespace AeroBites.Models.Admin
{
    public class DailySells
    {
        public required DateOnly Date { get; set; }
        public required int Amount { get; set; }
    }
}
