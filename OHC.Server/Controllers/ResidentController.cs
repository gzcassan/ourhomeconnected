using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OHC.Core;
using OHC.Core.Events;
using WhenDoJobs.Core.Interfaces;

namespace OHC.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/resident")]
    public class ResidentController : Controller
    {
        private IWhenDoQueueProvider queue;
        private ILogger<ResidentController> logger;

        public ResidentController(IWhenDoQueueProvider queue, ILogger<ResidentController> logger)
        {
            this.queue = queue;
            this.logger = logger;
        }

        [Authorize]
        [HttpPut("status/goingtosleep")]
        public void PutResidentStatus()
        {
            var user = User.Identity.Name;
            var messsage = new ResidentStatusEvent() { Status = ResidentsStatus.GoingToSleep };
            queue.EnqueueMessage(messsage);
        }
    }
}