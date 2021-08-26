using RabbitMQ.Client;
using System;
using System.Collections;
using System.IO;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            string filePath = @"C:\test";
            watcher.Path = filePath;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Filter = "*.*";

            watcher.IncludeSubdirectories = true;

            watcher.Created += new FileSystemEventHandler(OnChanged);

            new System.Threading.AutoResetEvent(false).WaitOne();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            var data = GetFileBits(path);

            SendMessage(path, data);
        }

        private static byte[] GetFileBits(string path)
        {
            return File.ReadAllBytes(path);
        }

        private static void SendMessage(string path, byte[] data)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.ExchangeDeclare("test", ExchangeType.Fanout, true, true);
            channel.BasicPublish("test", "", null, data);

            channel.Close();
            connection.Close();
        }
    }
}
