using System;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory(){ HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue:"task_queue", durable: true, exclusive: false, 
                        autoDelete: false, arguments: null);
                        
                    var message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "", routingKey: "task_queue", mandatory: false, 
                    basicProperties: properties, body: body);

                    Console.WriteLine($"[x] Sent {message}");
                }
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
        }

        static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "Hello World!";
        }
    }
}
