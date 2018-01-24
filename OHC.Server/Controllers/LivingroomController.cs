using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OHC.Core.AreaObservers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OHC.Server.Controllers
{
    [Route("api/[controller]")]
    public class LivingroomController : Controller
    {
        private ILivingroomObserver livingroomObserver;

        public LivingroomController(ILivingroomObserver livingroomObserver)
        {
            this.livingroomObserver = livingroomObserver;
        }
        // GET: api/<controller>
        [Authorize]
        [HttpGet("temperature")]
        public Double GetCurrentTemperature()
        {
            var user = User.Identity.Name;
            return livingroomObserver.GetCurrentTemperature();
        }
        
    }
}
