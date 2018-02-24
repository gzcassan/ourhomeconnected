using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OHC.Core;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OHC.Server.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [Authorize]
        [HttpPut("status/goingtosleep")]
        public void PostHomeStatus()
        {
            var user = User.Identity.Name;
            //eventAggregator.Publish<ResidentsStatusEvent>(new ResidentsStatusEvent(ResidentsStatus.GoingToSleep));
        }
    }
}
