namespace Configuration
{
    public class RateLimiterConfig
    {
        public static Policies Policy = new("fixed");
        public record Policies
        (
            string Fixed
        );
    }
}