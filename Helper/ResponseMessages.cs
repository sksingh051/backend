namespace Phase_07_Poc_01.Helper
{
    public static class ResponseMessages
    {
        // General
        public const string Success = "Operation completed successfully.";
        public const string Failure = "An unexpected error occurred.";
        public const string Unauthorized = "You do not have permission to perform this action.";

        // Cart
        public const string CartFetchSuccess = "Cart fetched successfully.";
        public const string CartEmpty = "Your cart is currently empty.";
        public const string CartItemAdded = "Product successfully added to your cart.";
        public const string CartItemUpdated = "Cart item updated successfully.";
        public const string CartItemDeleted = "Cart item deleted successfully.";
        public const string CartCleared = "Cart cleared successfully.";
        public const string CartItemNotFound = "Cart item not found or you do not have permission to modify it.";

        // Products
        public const string ProductNotFound = "The requested product does not exist.";
        public const string ProductsFetched = "Products fetched successfully.";
        public const string ProductCreated = "Product created successfully.";
        public const string ProductUpdated = "Product updated successfully.";
        public const string ProductDeleted = "Product deleted successfully.";

        // Users
        public const string UserNotFound = "The requested user does not exist.";
        public const string UsersFetched = "Users fetched successfully.";
        public const string UserCreated = "User created successfully.";
        public const string UserUpdated = "User updated successfully.";
        public const string UserDeleted = "User deleted successfully.";

        // Orders
        public const string OrderCreated = "Order placed successfully.";
        public const string OrdersFetched = "Orders fetched successfully.";
        public const string OrderNotFound = "The requested order does not exist.";
        public const string OrderStatusUpdated = "Order status updated successfully.";
        public const string CartEmptyForOrder = "Cannot place an order with an empty cart.";

        // Categories
        public const string CategoriesFetched = "Categories fetched successfully.";
        public const string CategoryNotFound = "The requested category does not exist.";
        public const string CategoryCreated = "Category created successfully.";
        public const string CategoryDeleted = "Category deleted successfully.";

        // Admin
        public const string SummaryFetched = "Summary fetched successfully.";

        // Auth
        public const string EmailAlreadyExists = "Email already exists.";
        public const string PasswordRequired = "Password is required.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string LoginSuccess = "Login successful.";

        // Notifications
        public const string NotificationCreated = "Notification created successfully.";
        public const string NotificationsFetched = "Notifications fetched successfully.";
        public const string NotificationNotFound = "Notification not found.";
        public const string NotificationMarkedRead = "Notification marked as read.";
    }
}
