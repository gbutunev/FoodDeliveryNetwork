namespace FoodDeliveryNetwork.Common
{
    public static class EntityConstants
    {
        public static class User
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 30;

            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 128;

            public const int PhoneNumberMinLength = 4;
            public const int PhoneNumberMaxLength = 15;

            public const int UsernameMinLength = 4;
            public const int UsernameMaxLength = 30;
        }
    }
}
