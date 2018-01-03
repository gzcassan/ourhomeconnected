using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Settings
{
    public class PhilipsHueSettings
    {
        public string Host { get; set; }
        public string Key { get; set; }
        public string SunsetScene { get; set; }

        public PhilipsHueSettings(string host, string key, string sunsetScene)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("Host");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("Key");

            if (string.IsNullOrEmpty(sunsetScene))
                throw new ArgumentNullException("SunsetScene");

            this.Host = host;
            this.Key = key;
            this.SunsetScene = sunsetScene;
        }
    }
}
