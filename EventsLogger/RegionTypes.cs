namespace Valuator
{
    public static class RegionTypes
    {
        public const string RUS = "rus";
        public const string EU = "eu";
        public const string OTHER = "other";

        public static readonly Dictionary<string, string> COUNTRY_TO_REGION = new()
        {
            ["russia"] = RUS,
            ["france"] = EU,
            ["germany"] = EU,
            ["usa"] = OTHER,
            ["india"] = OTHER
        };
    }
}
