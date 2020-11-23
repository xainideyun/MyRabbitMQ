using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public object Index([FromQuery] string msg, [FromQuery] string name)
        {
            name ??= "admin";
            msg ??= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Send(msg, name);
            return new { name, msg };
        }


        private void Send(string msg, string name)
        {
            var host = new { host = "81.68.253.181", username = "admin", password = "000000", queueName = "myqueue" };
            var factory = new ConnectionFactory() { HostName = host.host, UserName = host.username, Password = host.password, VirtualHost = "my_vhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                    queue: host.queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            channel.BasicPublish(
                    exchange: "",
                    routingKey: host.queueName,
                    basicProperties: null,
                    body: GetBuffer($"用户{name}发送消息：{msg}")
            );

        }

        private byte[] GetBuffer(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }
    }
}
