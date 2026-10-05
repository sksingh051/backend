namespace Phase_07_Poc_01.Static
{
    public sealed class ApiRoutes
    {
        public const string Api = "api";

        public class User
        {
            public const string UserBase = Api + "/user";
            public const string Profile = UserBase + "/me";
        }

        public class Auth
        {
            public const string AuthBase = Api + "/auth";
            public const string Register = AuthBase + "/register";
            public const string Login = AuthBase + "/login";
            public const string AdminLogin = AuthBase + "/admin/login";
        }

        public class Product
        {
            public const string ProductBase = Api + "/product";
            public const string GetProductById = ProductBase + "/{id}";
        }

        public class Category
        {
            public const string CategoryBase = Api + "/category";
            public const string GetCategoryById = CategoryBase + "/{id}";
            public const string DeleteCategory = CategoryBase + "/{id}";
        }

        public class Upload
        {
            public const string UploadBase = Api + "/upload";
        }

        public  class Cart
        {
            public const string CartBase = Api +"/cart";
            public const string GetCartById = CartBase + "/{itemId}";
        }

        public class Order
        {
            public const string OrderBase = Api + "/orders";
            public const string GetOrderById = OrderBase + "/{id}";
            public const string UpdateOrderStatus = OrderBase + "/{id}/status";
        }

        public class Payment
        {
            public const string SimulatePayment = Api + "/payments/simulate";
        }

        public class Notification
        {
            public const string NotificationBase = Api + "/notifications";
            public const string MarkAsRead = NotificationBase + "/{id}/read";
        }

        public class Admin
        {
            public const string Users = Api + "/admin/users";
            public const string OrdersSummary = Api + "/admin/orders/summary";
            public const string ProductsSummary = Api + "/admin/products/summary";
            public const string Orders = Api + "/admin/orders";
            public const string UpdateUserRole = Api + "/admin/users/{userId}/role";
        }
    }
}