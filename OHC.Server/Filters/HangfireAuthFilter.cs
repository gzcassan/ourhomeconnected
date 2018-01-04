using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHC.Server.Hangfire
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var host = context.GetHttpContext().Request.Host.Host;
            //only allow local network access
            //TODO: make more secure
            return host.StartsWith("192.168") || host.Equals("localhost");
        }
    }
}
