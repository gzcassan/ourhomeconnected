using Innovative.SolarCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OHC.Core
{
    public class OHCHelpers
    {
        public static DateTimeOffset CalculateSunset(DateTimeOffset date, string timezoneId, double lat, double lng)
        {
            var tz = TimeZoneInfo.GetSystemTimeZones().Single(x => x.Id == timezoneId);
            var time = new SolarTimes(date, lat, lng);
            DateTime sunset = TimeZoneInfo.ConvertTimeFromUtc(time.Sunset.ToUniversalTime(), tz);
            return (DateTimeOffset)sunset;
        }

        public static DateTimeOffset CalculateSunrise(DateTimeOffset date, string timezoneId, double lat, double lng)
        {
            var tz = TimeZoneInfo.GetSystemTimeZones().Single(x => x.Id == timezoneId);
            var time = new SolarTimes(date, lat, lng);
            DateTime sunrise = TimeZoneInfo.ConvertTimeFromUtc(time.Sunrise.ToUniversalTime(), tz);
            return (DateTimeOffset)sunrise;
        }
    }
}
