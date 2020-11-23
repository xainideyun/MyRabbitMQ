using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Htx.RabbitMQ.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 默认方法
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public object Index([FromQuery]string msg, [FromQuery]string name)
        {
            name ??= "admin";
            msg ??= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return new { name, msg };
        }
    }
}
