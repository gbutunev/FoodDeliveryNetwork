﻿namespace FoodDeliveryNetwork.Common
{
    public class AppConstants
    {
        public const string AppName = "Food Delivery Network";
        public const string NullRestaurant = "Deleted_Restaurant";
        public static class RoleNames
        {
            public const string AdministratorRole = "Administrator";
            public const string OwnerRole = "Owner";
            public const string CourierRole = "Courier";
            public const string DispatcherRole = "Dispatcher";
        }

        public static class NotificationTypes
        {
            public const string ErrorMessage = "error";
            public const string SuccessMessage = "success";
            public const string WarningMessage = "warning";
            public const string InfoMessage = "info";
        }
    }
}
