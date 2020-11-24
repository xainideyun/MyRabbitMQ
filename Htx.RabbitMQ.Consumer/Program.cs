using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Htx.RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Consumer();
            Subscribe();
        }

        
        /// <summary>
        /// 消费者
        /// </summary>
        static void Consumer()
        {
            var host = new { host = "81.68.253.181", username = "admin", password = "000000", queueName = "myqueue", vhost = "my_vhost" };
            var factory = new ConnectionFactory() { HostName = host.host, UserName = host.username, Password = host.password, VirtualHost = host.vhost };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(host.queueName, true, false, false, null);

            // 设置每次最多可以接收多少条消息，prefetchCount=1时，表明只有消息处理完成且返回确认后方可接收新消息
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                //Thread.Sleep(3000);
                channel.BasicAck(ea.DeliveryTag, false);    // 返回消息处理完成确认信息
            };
            channel.BasicConsume(
                queue: host.queueName,          // 队列名称
                autoAck: false,                 // 是否自动确认
                consumer: consumer              // 消费者
            );
            Console.ReadLine();
        }


        /// <summary>
        /// 订阅者
        /// </summary>
        static void Subscribe()
        {
            var host = new { host = "81.68.253.181", username = "admin", password = "000000", queueName = "myqueue", vhost = "my_vhost" };
            var factory = new ConnectionFactory() { HostName = host.host, UserName = host.username, Password = host.password, VirtualHost = host.vhost };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);           // 定义路由
            var queueName = channel.QueueDeclare().QueueName;               // 定义队列名称
            channel.QueueBind(queueName, "logs", "");                       // 绑定路由

            var subscribe = new EventingBasicConsumer(channel);
            subscribe.Received += (model, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };
            channel.BasicConsume(queueName, true, subscribe);
            Console.ReadLine();
        }

    }
}
