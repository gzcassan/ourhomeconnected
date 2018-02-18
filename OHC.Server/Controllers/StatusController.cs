using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OHC.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/status")]
    public class StatusController : Controller
    {
        public IActionResult Get()
        {
            return new OkResult();
        }
    }
}