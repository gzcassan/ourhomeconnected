using Microsoft.Extensions.Options;
using OHC.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using WhenDoJobs.Core.Interfaces;
using WhenDoJobs.Core.Providers;

namespace OHC.Core.Services
{
    public class OHCDateTimeProvider : IWhenDoExpressionProvider, IDateTimeProvider
    {
        private LocationSettings settings;

        public OHCDateTimeProvider(IOptions<LocationSettings> settings)
        {
            this.settings = settings.Value;
        }

        public TimeSpan Sunset => OHCHelpers.CalculateSunset(DateTimeOffset.Now, settings.TimezoneId, settings.Latitude, settings.Longitude).TimeOfDay;
        public TimeSpan Sunrise => OHCHelpers.CalculateSunrise(DateTimeOffset.Now, settings.TimezoneId, settings.Latitude, settings.Longitude).TimeOfDay;
        public TimeSpan CurrentTime => DateTimeOffset.Now.TimeOfDay;
        public DateTimeOffset CurrentDate => DateTimeOffset.Now.Date;
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
