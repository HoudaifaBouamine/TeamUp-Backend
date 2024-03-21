using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Configuration
{
    public class RateLimiterConfig
    {
        public static Policies Policy = new Policies("fixed");
        public record Policies(
            string Fixed
            );
    }
}