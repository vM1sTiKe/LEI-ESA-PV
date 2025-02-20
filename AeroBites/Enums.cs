namespace AeroBites
{
    public static class Enums
    {
        /// <summary>
        /// Represents the different pages within the application.
        /// </summary>
        public enum CurrentPage
        {
            Admin,
            Restaurant,
            RestaurantMenu,
            MyRestaurant,
        }

        /// <summary>
        /// Represents the various statuses a restaurant can have.
        /// </summary>
        public enum RestaurantStatus
        {
            WaitingAcceptance,
            Valid,
            Rejected
        }

        /// <summary>
        /// Represents the different statuses an order can go through.
        /// </summary>
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