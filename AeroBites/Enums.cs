namespace AeroBites
{
    public static class Enums
    {
        public enum RestaurantStatus
        {
            WaitingAcceptance,
            Valid,
            Rejected
        }

        public enum OrderStatus
        {
            Choosing,
            Placed,
            Preparing,
            OnTheWay,
            Waiting,
            Recieved
        }
    }
}