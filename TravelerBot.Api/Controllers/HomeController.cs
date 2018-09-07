using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TravelerBot.Api.Controllers
{
    [Route("api")]
    public class HomeController : Controller
    {
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }
    }
}
