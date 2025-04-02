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
            MyOrders,
            MyPayments,
            MyAddresses,
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

        /// <summary>
        /// Represents the different messages types.
        /// </summary>
        public enum MessageType
        {
            successMessage,
            errorMessage,
            infoMessage,
            warningMessage
        }
    }
}