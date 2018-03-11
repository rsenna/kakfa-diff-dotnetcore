using System;
using System.Linq;
using Autofac;
using Kafka.Diff.Common;
using Kafka.Diff.Subscriber.Autofac;
using Kafka.Diff.Subscriber.Handler;
using Nancy.Hosting.Self;

namespace Kafka.Diff.Subscriber
{
    /// <summary>
    /// Initializes the Subscriber application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var gitlab = args.Contains("--gitlab");

            if (gitlab)
            {
                SubscriberAutofacModule.KafkaServer = "spotify-kafka:9092";
            }

            // Initialize an instance of NancyHost:
            var configuration = new HostConfiguration {UrlReservations = new UrlReservations {CreateAutomatically = true}};
            var uri = new Uri("http://localhost:12340");

            Console.WriteLine($"Uri: {uri}.");

            var host = new NancyHost(configuration, uri);
            host.Start(); // start hosting

            if (!gitlab)
            {
                Console.ReadKey();
                host.Stop(); // stop hosting
            }
        }
    }
}
