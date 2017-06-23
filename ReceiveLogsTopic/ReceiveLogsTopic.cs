using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogsTopic
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_logs", "topic");
                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]} [binding_key...]");
                        Console.WriteLine("Press [enter] to exit");
                        Console.ReadLine();
                        Environment.Exit(1);
                        return;
                    }

                    foreach (var bindingKey in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
                    }

                    Console.WriteLine("[x] Waiting for messages. Press [Ctrl + C] to exit");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[x] Received '{ea.RoutingKey}': '{message}'");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine("Press [enter] to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
