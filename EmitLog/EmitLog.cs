﻿using System;
using System.Text;
using RabbitMQ.Client;

namespace EmitLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                    
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null,
                        body: body);
                    
                    Console.WriteLine($"[x] Sent {message}");
                    
                }
                Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
            }
        }

        static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
        }
    }
}
