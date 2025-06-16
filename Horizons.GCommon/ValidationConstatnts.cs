namespace Horizons.GCommon
{
    public static class ValidationConstatnts
    {
        public static class Destination 
        {
            public const int NameMaxLenght = 80;
            public const int NameMinLenght = 3;

            public const int DescriptionMaxLenght = 250;
            public const int DescriptionMinLenght = 10;

            public const string DateFormat = "dd-MM-yyyy";
        }

        public static class Terrain 
        {
            public const int NameMaxLenght = 20;
            public const int NameMinLenght = 3;
        }
    }
}
