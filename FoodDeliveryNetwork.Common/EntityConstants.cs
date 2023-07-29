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

        public static class OwnerConstants
        {
            public const int EIKMinLength = 9;
            public const int EIKMaxLength = 15;

            public const int FullNameMinLength = 8;
            public const int FullNameMaxLength = 92;

            public const int HeadquartersFullAddressMinLength = 8;
            public const int HeadquartersFullAddressMaxLength = 150;

            public const int CompanyNameMinLength = 2;
            public const int CompanyNameMaxLength = 50;

            public const int EGNLength = 10;
        }

        public static class CustomerConstants
        {
            public const int AddressMinLength = 8;
            public const int AddressMaxLength = 150;
        }

        public static class DishConstants
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;

            public const int DescriptionMinLength = 8;
            public const int DescriptionMaxLength = 150;
        }

        public static class RestaurantConstants
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;

            public const int DescriptionMinLength = 8;
            public const int DescriptionMaxLength = 500;

            public const int AddressMinLength = 8;
            public const int AddressMaxLength = 150;

            public const int PhoneNumberMinLength = 4;
            public const int PhoneNumberMaxLength = 15;
        }
    }
}
