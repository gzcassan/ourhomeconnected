using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHC.Server.Auth

{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "apiKey";
        public string Scheme => DefaultScheme;
        public Dictionary<string, string> Account { get; set; }

        public CustomAuthOptions()
        {
            Account = new Dictionary<string, string>();
        }
    }
}

