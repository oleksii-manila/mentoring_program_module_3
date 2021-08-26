using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.QueueDeclare("testQueue", true, false, false);
            channel.QueueBind("testQueue", "test", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                string Name = @"C:\testServer\test.pdf";

                var Writer = new BinaryWriter(File.OpenWrite(Name));

                Writer.Write(args.Body.ToArray());
                Writer.Flush();
                Writer.Close();
            };

            channel.BasicConsume("testQueue", true, consumer);

            Console.ReadLine();
            channel.Close();
            connection.Close();
        }
    }
}
