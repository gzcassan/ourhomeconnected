using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Drivers.NefitEasy
{
    public class NefitEasyClient : INefitEasyClient
    {
        NefitEasySettings settings;

        public NefitEasyClient(NefitEasySettings settings)
        {
            this.settings = settings;
        }

        public async Task SetScheduleOverruleTempAsync(double temp, int durationInMinutes = 0)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{settings.Host}:{settings.Port}/bridge/heatingCircuits/hc1/manualTempOverride/");

            //set temperature
            var tempJson = new JObject();
            tempJson.Add(new JProperty("value", temp));
            var tempContent = new StringContent(tempJson.ToString(), Encoding.UTF8, "application/json");
            await httpClient.PostAsync("temperature", tempContent);

            //set duration
            if (durationInMinutes > 0)
            {
                var durationJson = new JObject();
                durationJson.Add(new JProperty("value", durationInMinutes));
                var durationContent = new StringContent(durationJson.ToString(), Encoding.UTF8, "application/json");
                await httpClient.PostAsync("duration", durationContent);
            }

            //set status
            var statusJson = new JObject();
            statusJson.Add(new JProperty("value", "on"));
            var statusContent = new StringContent(statusJson.ToString(), Encoding.UTF8, "application/json");
            await httpClient.PostAsync("status", statusContent);
        }
    }
}
