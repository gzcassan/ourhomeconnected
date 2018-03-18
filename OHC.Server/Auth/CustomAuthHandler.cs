using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace OHC.Server.Auth
{
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Path.StartsWithSegments(PathString.FromUriComponent("/api")))
                return Task.FromResult(AuthenticateResult.NoResult());
            var username = Request.Query.FirstOrDefault(x => x.Key == "user").Value;
            var secretKey = Request.Query.FirstOrDefault(x => x.Key == "key").Value;

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(secretKey))
            {

                string secret = String.Empty;
                if (Options.Account.TryGetValue(username, out secret))
                {
                    if (secret == secretKey)
                    {
                        var user = new GenericPrincipal(new GenericIdentity(username), null);
                        var identities = new List<ClaimsIdentity> { new ClaimsIdentity("apiKey") };
                        var ticket = new AuthenticationTicket(user, Options.Scheme);
                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                }
            }
            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials."));
        }
    }
}
